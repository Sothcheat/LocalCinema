using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalCinema.Models;

namespace LocalCinema.ViewModels
{
    public partial class MovieDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Movie? _movie;

        [ObservableProperty]
        private bool _isVisible;

        public void ShowDetails(Movie movie)
        {
            Movie = movie;
            IsVisible = true;
        }

        [RelayCommand]
        private void Close()
        {
            IsVisible = false;
            Movie = null;
        }

        [RelayCommand]
        private void Play()
        {
            // Trigger play event
        }
    }
}