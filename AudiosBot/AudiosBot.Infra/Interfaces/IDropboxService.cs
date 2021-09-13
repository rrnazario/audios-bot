using System.Threading.Tasks;

namespace AudiosBot.Infra.Interfaces
{
    public interface IDropboxService
    {
        /// <summary>
        /// Get the base64 image code from Dropbox.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        Task<byte[]> GetAudioContentAsync(string uid);
        Task<bool> UploadAudioAsync(string name, byte[] content);
        Task<bool> RemoveAudioAsync(string name);
    }
}
