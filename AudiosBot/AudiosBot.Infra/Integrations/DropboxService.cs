using AudiosBot.Infra.Constants;
using AudiosBot.Infra.Helpers;
using AudiosBot.Infra.Interfaces;
using AudiosBot.Infra.Models;
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
        private readonly string Token = AdminConstants.DropboxToken;

        public DropboxService()
        {
            using var client = new DropboxClient(Token);
            try
            {
                client.Files.CreateFolderV2Async(_path).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                //log
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        public async Task<List<AudioFile>> GetAudioContentAsync(string term)
        {
            LogHelper.Debug($"{nameof(GetAudioContentAsync)}");

            using var client = new DropboxClient(Token);
            var result = new List<AudioFile>();

            var list = await client.Files.ListFolderAsync(_path);

            var search = list.Entries.Where(wh => wh.Name.Contains(term, StringComparison.OrdinalIgnoreCase));            

            var metadatas = new List<Metadata>();

            if (search.Any() || list.HasMore)
            {
                metadatas.AddRange(search);

                while (list.HasMore && !search.Any())
                {
                    list = await client.Files.ListFolderContinueAsync(list.Cursor);

                    metadatas.AddRange(list.Entries.Where(i => i.IsFile && i.Name.Equals(term)));
                }

                foreach (var metadata in metadatas)
                {
                    var fileName = metadata.AsFile.Name;

                    using var response = await client.Files.DownloadAsync(metadata.AsFile.PathLower);

                    result.Add(new() { Content = await response.GetContentAsByteArrayAsync(), Name = fileName });
                }

                LogHelper.Debug($"Found.");
            }
            
            return result;
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
