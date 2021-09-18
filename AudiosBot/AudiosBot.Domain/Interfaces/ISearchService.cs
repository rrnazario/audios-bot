using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudiosBot.Domain.Interfaces
{
    public interface ISearchService
    {
        /// <summary>
        /// Downloads the audios to a temp folder, in order to be erased when the sending operation is done.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="currentSearchFolder"></param>
        /// <returns></returns>
        Task<List<string>> GetMatchedAudiosAsync(string term, string currentSearchFolder);
    }
}
