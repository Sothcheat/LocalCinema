using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalCinema.Models;
using LocalCinema.Services;

namespace LocalCinema.ViewModels
{
    public partial class LibraryViewModel : ObservableObject
    {
        private readonly IMovieRepository _movieRepository;
        private List<Movie> _allMovies = new();

        [ObservableProperty]
        private ObservableCollection<Movie> _movies = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private Movie? _selectedMovie;

        public LibraryViewModel(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task LoadMoviesAsync()
        {
            IsLoading = true;
            _allMovies = await _movieRepository.GetAllMoviesAsync();
            Movies = new ObservableCollection<Movie>(_allMovies);
            IsLoading = false;
        }

        partial void OnSearchQueryChanged(string value)
        {
            FilterMovies();
        }

        private void FilterMovies()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Movies = new ObservableCollection<Movie>(_allMovies);
            }
            else
            {
                var filtered = _allMovies.Where(m =>
                    m.Title.Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
                Movies = new ObservableCollection<Movie>(filtered);
            }
        }

        [RelayCommand]
        private void SortByTitle()
        {
            Movies = new ObservableCollection<Movie>(Movies.OrderBy(m => m.Title));
        }

        [RelayCommand]
        private void SortByYear()
        {
            Movies = new ObservableCollection<Movie>(Movies.OrderByDescending(m => m.Year));
        }

        [RelayCommand]
        private void SelectMovie(Movie movie)
        {
            SelectedMovie = movie;
        }
    }
}