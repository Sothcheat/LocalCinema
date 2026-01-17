using System.Collections.Generic;
using System.Threading.Tasks;
using LocalCinema.Models;

namespace LocalCinema.Services
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(int id);
        Task<Movie> AddMovieAsync(Movie movie);
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(int id);
        Task<List<Movie>> SearchMoviesAsync(string query);
        Task<bool> MovieExistsAsync(string filePath);
    }
}