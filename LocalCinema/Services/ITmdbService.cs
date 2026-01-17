using System.Threading.Tasks;
using LocalCinema.Models;

namespace LocalCinema.Services
{
    public interface ITmdbService
    {
        Task<TmdbMovieDetails?> SearchMovieAsync(string title, int? year = null);
        Task<TmdbCredits?> GetMovieCreditsAsync(int tmdbId);
        string GetPosterUrl(string? posterPath, string size = "w500");
        string GetBackdropUrl(string? backdropPath, string size = "w1280");
    }
}