using AudiosBot.Domain.Interfaces;
using AudiosBot.Domain.Models;
using AudiosBot.Infra.Constants;
using AudiosBot.Infra.Helpers;
using AudiosBot.Infra.Interfaces;
using AudiosBot.Infra.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace AudiosBot.Domain.Services
{
    public class UploadService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource cts = new();
        private readonly TelegramBotClient _bot;
        private readonly IDropboxService _dropboxService;
        private readonly ICommandService _commandService;
        private Task _executingTask;

        private readonly List<AudioFile> sentAudios = new();

        public UploadService(IDropboxService dropboxService, ICommandService commandService)
        {
            _bot = new TelegramBotClient(AdminConstants.TelegramBotToken);
            _dropboxService = dropboxService;
            _commandService = commandService;
        }

        public void Dispose()
        {
            cts.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _bot.OnMessage += botOnMessage;
            _bot.StartReceiving(cancellationToken: cancellationToken);

            _executingTask = MonitorUploadAsync(cts.Token);

            return _executingTask.IsCompleted
                ? _executingTask
                : Task.CompletedTask;
        }

        private void botOnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Audio ||
                e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                //e.Message.Audio.FileId
                var file = _bot.GetFileAsync(e.Message.Document?.FileId ?? e.Message.Audio?.FileId).GetAwaiter().GetResult();
                using var fs = new FileStream(e.Message.Document.FileName, FileMode.Create);
                using BinaryReader binaryReader = new BinaryReader(fs);
                fs.Position = 0;

                _bot.DownloadFileAsync(file.FilePath, fs).GetAwaiter().GetResult();
                fs.Position = 0;

                sentAudios.Add(new() { Content = binaryReader.ReadBytes((int)fs.Length), Extension = file.FilePath.Split(".").Last() });
                _bot.SendTextMessageAsync(e.Message.Chat.Id, "Enviar o nome em seguida (não enviar outro arquivo).").GetAwaiter().GetResult();

                SystemHelper.DeleteFile(e.Message.Document.FileName);
            }
            else
            {
                var choosenAudio = sentAudios.LastOrDefault();
                if (!string.IsNullOrEmpty(e.Message.Text) && TelegramHelper.UserIsAdmin(e.Message.Chat.Id.ToString()) && choosenAudio != null)
                {
                    var uploadOk = _dropboxService.UploadAudioAsync($"{e.Message.Text}.{choosenAudio.Extension}", choosenAudio.Content).GetAwaiter().GetResult();
                    if (uploadOk)
                        _bot.SendTextMessageAsync(e.Message.Chat.Id, "Upload feito com sucesso.").GetAwaiter().GetResult();
                    else
                        _bot.SendTextMessageAsync(e.Message.Chat.Id, "Erro no upload feito.").GetAwaiter().GetResult();

                    sentAudios.Clear();
                }
                else
                    _commandService.SearchAsync(new()
                    {
                        Term = e.Message.Text,
                        User = new(e.Message.Chat.FirstName,
                                   e.Message.Chat.LastName,
                                   e.Message.Chat.Id.ToString(),
                                   e.Message.Chat.Id.ToString())
                    });
            }            
        }

        private async Task MonitorUploadAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromMinutes(5), token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
                return;

            try
            {
                cts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }
    }
}
