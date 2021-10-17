using AudiosBot.Infra.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using static AudiosBot.Infra.Constants.Enums;

namespace AudiosBot.Infra.Helpers
{
    public class TelegramHelper
    {
        public static async Task<SendFileResult> SendFileAsync(TelegramBotClient bot, string filePath, string chatId, string adminChatId)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                var fileinfo = new FileInfo(filePath);

                await bot.SendAudioAsync(chatId, new InputOnlineFile(stream, fileinfo.Name));

                return SendFileResult.SUCCESS;
            }
            catch (Exception exc)
            {
                //LogHelper.Info($"Caminho da imagem: {filePath}\n\n*{exc.Message}*\n\n{exc.StackTrace}", true);

                return exc.Message.Contains("bot was blocked by the user", StringComparison.InvariantCultureIgnoreCase)
                    ? SendFileResult.BLOCKED_BY_USER
                    : SendFileResult.GENERAL_ERROR;
            }
        }

        public static async Task SafeSendFilesAsync(TelegramBotClient bot, IEnumerable<string> items, string chatId, string folder, string adminChatId)
        {
            int count = 1, total = items.Count();
            foreach (var item in items)
            {
                Console.WriteLine($"[{count} de {total}] Enviando: {item}");

                var result = await SendFileAsync(bot, item, chatId, adminChatId);

                if (result == SendFileResult.BLOCKED_BY_USER) //To prevent sending many erros to admin.
                    break;

                count++;

                Thread.Sleep(TimeSpan.FromSeconds(1)); //To prevent 429 error, Telegram FAQ said to send 1 info per second.
            };

            try
            {
                folder = GetFolderToDelete(items, folder);

                SystemHelper.DeleteFolder(folder);
            }
            catch (Exception exc)
            {
                //LogHelper.Error(exc);
            }
        }

        public static InlineKeyboardMarkup GenerateKeyboard(IEnumerable<(string text, string url)> buttons)
        {
            List<List<InlineKeyboardButton>> inlineButtons = default;
            if (buttons.Any())
            {
                inlineButtons = new List<List<InlineKeyboardButton>>();
                foreach (var button in buttons)
                {
                    var inlineButton = Uri.IsWellFormedUriString(button.url, UriKind.Absolute)
                                       ? new InlineKeyboardButton() { Text = button.text, Url = button.url }
                                       : new InlineKeyboardButton() { Text = button.text, CallbackData = button.url };

                    inlineButtons.Add(new() { inlineButton });
                }
            }

            return new InlineKeyboardMarkup(inlineButtons);
        }

        private static string GetFolderToDelete(IEnumerable<string> messages, string folder)
        {
            if (messages.Any(a => !string.IsNullOrEmpty(a)))
            {
                var file = messages.FirstOrDefault(f => !f.StartsWith("http", StringComparison.InvariantCultureIgnoreCase));

                return (!string.IsNullOrEmpty(file) && File.Exists(file)) ? new FileInfo(file).DirectoryName : Path.Combine(Directory.GetCurrentDirectory(), folder);
            }
            else
                return Path.Combine(Directory.GetCurrentDirectory(), folder);
        }

        public static bool UserIsAdmin(string Id) => AdminConstants.AdminChatId.Split(",").Contains(Id);       
    }
}
