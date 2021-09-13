using AudiosBot.Domain.Interfaces;
using AudiosBot.Domain.Models;
using AudiosBot.Infra.Constants;
using AudiosBot.Infra.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace AudiosBot.Domain.Services
{
    public class CommandService : ICommandService
    {
        private readonly TelegramBotClient _bot;
        private readonly ISearchService _searchService;

        public CommandService(ISearchService searchService)
        {
            _bot = new TelegramBotClient(AdminConstants.TelegramBotToken);
            _searchService = searchService;
        }

        public async Task DefineAsync(Search search)
        {
            if (search.Term.Length <= 1)
            {
                var bot = new TelegramBotClient(AdminConstants.TelegramBotToken);
                await bot.SendTextMessageAsync(search.User.Id, "Você pesquisou um termo muito pequeno. Experimente pesquisar uma palavra maior.");

                return;
            }

            if (search.User.IsAdmin)
            {

            }
            else
                await SearchAsync(search);
        }

        private async Task SearchAsync(Search search)
        {
            await _bot.SendTextMessageAsync(search.User.Id, MessageHelper.GetRandomSearchingMessage());

            var currentSearchFolder = $"{search.User.Id}/{DateTime.Now:ddMMyyyyHHmmssfff}";

            var audios = await _searchService.GetMatchedAudiosAsync(search.Term, currentSearchFolder);

            await TelegramHelper.SafeSendFilesAsync(_bot, audios, search.User.Id, currentSearchFolder, AdminConstants.AdminChatId);
        }
    }
}
