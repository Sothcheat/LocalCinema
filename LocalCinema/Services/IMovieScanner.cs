using System.Collections.Generic;
using System.Threading.Tasks;
using LocalCinema.Models;

namespace LocalCinema.Services
{
    public interface IMovieScanner
    {
        Task<List<Movie>> ScanFolderAsync(string folderPath);
        bool IsVideoFile(string filePath);
    }
}