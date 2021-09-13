using AudiosBot.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace AudiosBot.Domain.Services
{
    public class SearchService : ISearchService
    {        
        public Task<string[]> GetMatchedAudiosAsync(string term, string currentSearchFolder)
        {
            throw new NotImplementedException();
        }
    }
}
