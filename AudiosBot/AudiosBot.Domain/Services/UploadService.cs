using AudiosBot.Domain.Interfaces;
using AudiosBot.Infra.Constants;
using AudiosBot.Infra.Extensions;
using AudiosBot.Infra.Helpers;
using AudiosBot.Infra.Interfaces;
using AudiosBot.Infra.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;

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
            _bot.DeleteWebhookAsync().Wait();

            _dropboxService = dropboxService;
            _commandService = commandService;
        }

        public void Dispose()
        {
            cts.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            LogHelper.Debug($"HostedService StartAsync.");

            _executingTask = MonitorUploadAsync(cts.Token);

            return _executingTask.IsCompleted
                ? _executingTask
                : Task.CompletedTask;
        }

        private async Task MonitorUploadAsync(CancellationToken token)
        {
            _bot.StartReceiving(new UpdateHandler(sentAudios, _dropboxService, _commandService), token);

            while (!token.IsCancellationRequested)
            {
                LogHelper.Debug($"HostedService MAIN THREAD.");
                await Task.Delay(TimeSpan.FromMinutes(3), token);
            }
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

    class UpdateHandler : IUpdateHandler
    {
        private readonly List<AudioFile> sentAudios;
        private readonly IDropboxService _dropboxService;
        private readonly ICommandService _commandService;
        public UpdateHandler(List<AudioFile> audioFiles, IDropboxService dropboxService, ICommandService commandService)
        {
            sentAudios = audioFiles;
            _dropboxService = dropboxService;
            _commandService = commandService;
        }

        public UpdateType[] AllowedUpdates => new UpdateType[] { };

        public async Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                await Task.Run(() => LogHelper.Error(apiRequestException));
            }
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            LogHelper.Debug($"Polling bot received.");

            if (!update.Message.IsUserAdmin()) 
            {
                LogHelper.Debug($"Regular user searching...");
                await _commandService.SearchAsync(new(update.Message));
                return; 
            }

            if (update.Message.IsValidAudioFile())
            {
                //e.Message.Audio.FileId
                LogHelper.Debug($"HostedService File received.");
                var file = await botClient.GetFileAsync(update.Message.GetFileId());
                using var fs = new FileStream(update.Message.GetFileName(), FileMode.Create);
                using BinaryReader binaryReader = new BinaryReader(fs);
                fs.Position = 0;

                await botClient.DownloadFileAsync(file.FilePath, fs);
                fs.Position = 0;

                sentAudios.Add(new() { Content = binaryReader.ReadBytes((int)fs.Length), Extension = file.FilePath.Split(".").Last(), OwnerId = update.Message.Chat.Id });
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Enviar o nome em seguida (não enviar outro arquivo).");

                SystemHelper.DeleteFile(update.Message.GetFileName());
            }
            else
            {
                var choosenAudio = sentAudios.FirstOrDefault(f => f.OwnerId == update.Message.Chat.Id);
                if (!string.IsNullOrEmpty(update.Message.Text) &&
                    TelegramHelper.UserIsAdmin(update.Message.Chat.Id.ToString()) && 
                    choosenAudio != null)
                {
                    if (update.Message.Text.Length > 255)
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"*ERRO*\n\nEnvie um nome mais curto. Tamanho atual: {update.Message.Text.Length}.", ParseMode.Markdown);
                        return;
                    }
                    
                    LogHelper.Debug($"HostedService audio title received");
                    var uploadOk = await _dropboxService.UploadAudioAsync($"{update.Message.Text}.{choosenAudio.Extension}", choosenAudio.Content);
                    if (uploadOk)
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Upload feito com sucesso.");
                    else
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Erro no upload feito.");

                    sentAudios.Remove(choosenAudio);
                }
                else
                {
                    LogHelper.Debug($"HostedService admin searching...");
                    await _commandService.SearchAsync(new(update.Message));
                }
            }
        }
    }
}
