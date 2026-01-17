namespace LocalCinema.Models
{
    public enum MediaSourceType
    {
        Local,
        Network
    }

    public class MediaSource
    {
        public MediaSourceType Type { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
}