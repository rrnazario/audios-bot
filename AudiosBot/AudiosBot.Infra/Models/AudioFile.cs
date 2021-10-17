namespace AudiosBot.Infra.Models
{
    public class AudioFile
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public long OwnerId { get; set; }
        public byte[] Content { get; set; }
    }
}
