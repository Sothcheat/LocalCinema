using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LocalCinema.Models;

namespace LocalCinema.Services
{
    public class MovieScanner : IMovieScanner
    {
        private readonly IMovieRepository _repository;
        private readonly IMetadataProvider _metadataProvider;
        private static readonly string[] VideoExtensions = { ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".m4v", ".webm" };

        public MovieScanner(IMovieRepository repository, IMetadataProvider metadataProvider)
        {
            _repository = repository;
            _metadataProvider = metadataProvider;
        }

        public async Task<List<Movie>> ScanFolderAsync(string folderPath)
        {
            var movies = new List<Movie>();

            if (!Directory.Exists(folderPath))
                return movies;

            var videoFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(f => IsVideoFile(f))
                .ToList();

            foreach (var filePath in videoFiles)
            {
                if (await _repository.MovieExistsAsync(filePath))
                    continue;

                try
                {
                    var movie = await _metadataProvider.ExtractMetadataAsync(filePath);
                    movies.Add(movie);
                    await _repository.AddMovieAsync(movie);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error scanning {filePath}: {ex.Message}");
                }
            }

            return movies;
        }

        public bool IsVideoFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return VideoExtensions.Contains(extension);
        }
    }
}