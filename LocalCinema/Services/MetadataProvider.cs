using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LocalCinema.Models;

namespace LocalCinema.Services
{
    public class MetadataProvider : IMetadataProvider
    {
        private readonly ITmdbService _tmdbService;

        public MetadataProvider(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        public async Task<Movie> ExtractMetadataAsync(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var fileInfo = new FileInfo(filePath);

            var title = CleanTitle(fileName);
            var year = ExtractYear(fileName);

            // Search TMDb for movie metadata
            var tmdbMovie = await _tmdbService.SearchMovieAsync(title, year > 0 ? year : null);

            if (tmdbMovie != null)
            {
                // Get cast information
                var credits = await _tmdbService.GetMovieCreditsAsync(tmdbMovie.Id);
                var cast = credits?.Cast.Take(5).Select(c => c.Name).ToList();

                return new Movie
                {
                    Title = tmdbMovie.Title,
                    FilePath = filePath,
                    Year = !string.IsNullOrEmpty(tmdbMovie.ReleaseDate) && tmdbMovie.ReleaseDate.Length >= 4
                        ? int.Parse(tmdbMovie.ReleaseDate.Substring(0, 4))
                        : year,
                    ReleaseDate = tmdbMovie.ReleaseDate,
                    Description = tmdbMovie.Overview,
                    Genre = string.Join(", ", tmdbMovie.Genres.Select(g => g.Name)),
                    Runtime = tmdbMovie.Runtime,
                    Cast = cast != null ? string.Join(", ", cast) : string.Empty,
                    PosterPath = _tmdbService.GetPosterUrl(tmdbMovie.PosterPath),
                    BackdropPath = _tmdbService.GetBackdropUrl(tmdbMovie.BackdropPath),
                    VoteAverage = tmdbMovie.VoteAverage,
                    TmdbId = tmdbMovie.Id,
                    DateAdded = DateTime.Now,
                    FileSize = fileInfo.Length,
                    IsLocal = true
                };
            }

            // Fallback if TMDb search fails
            return new Movie
            {
                Title = title,
                FilePath = filePath,
                Year = year,
                DateAdded = DateTime.Now,
                FileSize = fileInfo.Length,
                IsLocal = true,
                Genre = "Unknown",
                Description = "No description available.",
                PosterPath = "ms-appx:///Assets/placeholder-poster.png"
            };
        }

        private string CleanTitle(string fileName)
        {
            // Remove year
            fileName = Regex.Replace(fileName, @"\(\d{4}\)", "");
            fileName = Regex.Replace(fileName, @"\d{4}", "");

            // Remove common tags
            fileName = Regex.Replace(fileName, @"\[.*?\]", "");
            fileName = Regex.Replace(fileName, @"\(.*?\)", "");

            // Replace dots and underscores with spaces
            fileName = fileName.Replace(".", " ").Replace("_", " ");

            // Remove quality indicators
            var qualityPatterns = new[] { "1080p", "720p", "480p", "4K", "BluRay", "WEB-DL", "HDRip", "x264", "x265", "HEVC" };
            foreach (var pattern in qualityPatterns)
            {
                fileName = fileName.Replace(pattern, "", StringComparison.OrdinalIgnoreCase);
            }

            return fileName.Trim();
        }

        private int ExtractYear(string fileName)
        {
            var yearMatch = Regex.Match(fileName, @"\b(19|20)\d{2}\b");
            if (yearMatch.Success && int.TryParse(yearMatch.Value, out int year))
            {
                return year;
            }
            return 0;
        }
    }
}