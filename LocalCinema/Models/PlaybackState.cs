using System;

namespace LocalCinema.Models
{
    public class PlaybackState
    {
        public bool IsPlaying { get; set; }
        public TimeSpan CurrentTime { get; set; }
        public TimeSpan Duration { get; set; }
        public float Volume { get; set; } = 1.0f;
        public bool IsMuted { get; set; }
    }
}