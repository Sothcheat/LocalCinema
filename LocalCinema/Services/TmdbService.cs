using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LocalCinema.Models;

namespace LocalCinema.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.themoviedb.org/3";
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p";

        public TmdbService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<TmdbMovieDetails?> SearchMovieAsync(string title, int? year = null)
        {
            try
            {
                // First, search for the movie
                var searchUrl = $"{BaseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(title)}";
                if (year.HasValue)
                {
                    searchUrl += $"&year={year}";
                }

                var searchResponse = await _httpClient.GetStringAsync(searchUrl);
                var searchResult = JsonSerializer.Deserialize<TmdbSearchResult>(searchResponse);

                if (searchResult?.Results == null || searchResult.Results.Count == 0)
                    return null;

                // Get the first result's details
                var movieId = searchResult.Results[0].Id;
                return await GetMovieDetailsAsync(movieId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TMDb search error: {ex.Message}");
                return null;
            }
        }

        private async Task<TmdbMovieDetails?> GetMovieDetailsAsync(int movieId)
        {
            try
            {
                var detailsUrl = $"{BaseUrl}/movie/{movieId}?api_key={_apiKey}";
                var detailsResponse = await _httpClient.GetStringAsync(detailsUrl);
                return JsonSerializer.Deserialize<TmdbMovieDetails>(detailsResponse);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TMDb details error: {ex.Message}");
                return null;
            }
        }

        public async Task<TmdbCredits?> GetMovieCreditsAsync(int tmdbId)
        {
            try
            {
                var creditsUrl = $"{BaseUrl}/movie/{tmdbId}/credits?api_key={_apiKey}";
                var creditsResponse = await _httpClient.GetStringAsync(creditsUrl);
                return JsonSerializer.Deserialize<TmdbCredits>(creditsResponse);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TMDb credits error: {ex.Message}");
                return null;
            }
        }

        public string GetPosterUrl(string? posterPath, string size = "w500")
        {
            if (string.IsNullOrEmpty(posterPath))
                return "ms-appx:///Assets/placeholder-poster.png";

            return $"{ImageBaseUrl}/{size}{posterPath}";
        }

        public string GetBackdropUrl(string? backdropPath, string size = "w1280")
        {
            if (string.IsNullOrEmpty(backdropPath))
                return string.Empty;

            return $"{ImageBaseUrl}/{size}{backdropPath}";
        }
    }
}