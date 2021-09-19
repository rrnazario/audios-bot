using AudiosBot.Domain.Interfaces;
using AudiosBot.Infra.Helpers;
using AudiosBot.Infra.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AudiosBot.Domain.Services
{
    public class SearchService : ISearchService
    {
        private readonly IDropboxService _dropboxService;

        public SearchService(IDropboxService dropboxService)
        {
            _dropboxService = dropboxService;
        }

        public async Task<List<string>> GetMatchedAudiosAsync(string term, string currentSearchFolder)
        {
            var audios = await _dropboxService.GetAudioContentAsync(term);
            var result = new List<string>();

            if (audios.Any() && !Directory.Exists(currentSearchFolder))
                Directory.CreateDirectory(currentSearchFolder);

            LogHelper.Debug($"Downloading to local...");

            foreach (var audio in audios)
            {                
                var fullAudioName = $"{currentSearchFolder}\\{audio.Name}";
                await File.WriteAllBytesAsync(fullAudioName, audio.Content);
                result.Add(fullAudioName);                
            }

            LogHelper.Debug($"Done.");

            return result;
        }
    }
}
