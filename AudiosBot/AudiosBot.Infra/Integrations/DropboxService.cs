using AudiosBot.Infra.Interfaces;
using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AudiosBot.Infra.Integrations
{
    public class DropboxService : IDropboxService
    {
        private readonly string _path = "/Audios";
        private readonly string Token = Environment.GetEnvironmentVariable("DropboxToken");
        public async Task<byte[]> GetAudioContentAsync(string uid)
        {
            using var client = new DropboxClient(Token);

            var list = await client.Files.ListFolderAsync(_path);

            var search = list.Entries.Where(wh => wh.Name.Equals(uid));
            var metadatas = new List<Metadata>();

            if (search.Any() || list.HasMore)
            {
                metadatas.AddRange(search);

                while (list.HasMore && !search.Any())
                {
                    list = await client.Files.ListFolderContinueAsync(list.Cursor);

                    metadatas.AddRange(list.Entries.Where(i => i.IsFile && i.Name.Equals(uid)));
                }

                foreach (var metadata in metadatas)
                {
                    var fileName = metadata.AsFile.Name;

                    using (var response = await client.Files.DownloadAsync(metadata.AsFile.PathLower))
                    {
                        return await response.GetContentAsByteArrayAsync();
                    }
                }
            }

            return null;
        }

        public async Task<bool> RemoveAudioAsync(string name)
        {
            using var client = new DropboxClient(Token);
            await client.Files.DeleteV2Async($"{_path}/{name}");

            return true;
        }

        public async Task<bool> UploadAudioAsync(string name, byte[] content)
        {
            try
            {
                using var stream = new MemoryStream(content);
                stream.Position = 0;

                using var client = new DropboxClient(Token);
                await client.Files.UploadAsync(new CommitInfo($"{_path}/{name}", WriteMode.Overwrite.Instance), stream);

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"[UploadImageAsync] ==> {e.Message}\n\n{e.StackTrace}");

                return false;
            }
        }
    }
}
