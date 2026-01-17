using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalCinema.Services;
using System.Threading.Tasks;

namespace LocalCinema.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMovieScanner _movieScanner;

        [ObservableProperty]
        private string _title = "LocalCinema";

        public MainViewModel(IMovieRepository movieRepository, IMovieScanner movieScanner)
        {
            _movieRepository = movieRepository;
            _movieScanner = movieScanner;
        }

        [RelayCommand]
        private async Task ScanLibraryAsync()
        {
            // Open folder picker and scan
            var folder = @"C:\Movies"; // Placeholder
            await _movieScanner.ScanFolderAsync(folder);
        }
    }
}