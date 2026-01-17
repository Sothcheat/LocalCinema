using System.Threading.Tasks;
using LocalCinema.Models;

namespace LocalCinema.Services
{
    public interface IMetadataProvider
    {
        Task<Movie> ExtractMetadataAsync(string filePath);
    }
}