using System;
using System.ComponentModel.DataAnnotations;

namespace LocalCinema.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string PosterPath { get; set; } = string.Empty;
        public string BackdropPath { get; set; } = string.Empty;
        public int Year { get; set; }
        public string ReleaseDate { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Runtime { get; set; } // in minutes
        public string Cast { get; set; } = string.Empty; // Comma-separated
        public long FileSize { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? LastWatched { get; set; }
        public string VideoCodec { get; set; } = string.Empty;
        public string AudioCodec { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
        public double VoteAverage { get; set; }
        public int TmdbId { get; set; }

        // Network streaming
        public bool IsLocal { get; set; } = true;
        public string? StreamUrl { get; set; }
    }
}