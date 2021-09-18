namespace AudiosBot.Infra.Models
{
    public class AudioFile
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] Content { get; set; }
    }
}
