using AudiosBot.Infra.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudiosBot.Infra.Interfaces
{
    public interface IDropboxService
    {
        /// <summary>
        /// Get the base64 image code from Dropbox.
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        Task<List<AudioFile>> GetAudioContentAsync(string term);
        Task<bool> UploadAudioAsync(string name, byte[] content);
        Task<bool> RemoveAudioAsync(string name);
    }
}
