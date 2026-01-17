using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using LocalCinema.ViewModels;
using LocalCinema.Models;
using LibVLCSharp.Shared;
using System;
using Windows.System;
using System.Runtime.InteropServices;

namespace LocalCinema.Views
{
    public sealed partial class LibraryPage : Page
    {
        public LibraryViewModel ViewModel { get; }
        private LibVLC? _libVLC;
        private MediaPlayer? _mediaPlayer;
        private Movie? _currentMovie;
        private bool _isVideoPlaying;

        public LibraryPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<LibraryViewModel>();

            // Initialize LibVLC
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            Loaded += LibraryPage_Loaded;
            KeyDown += LibraryPage_KeyDown;
        }

        private async void LibraryPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadMoviesAsync();
        }

        private void LibraryPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                if (PlayerOverlay.Visibility == Visibility.Visible)
                {
                    ClosePlayer_Click(null, null);
                }
                else if (DetailsOverlay.Visibility == Visibility.Visible)
                {
                    CloseDetails_Click(null, null);
                }
            }
            else if (e.Key == VirtualKey.Space && PlayerOverlay.Visibility == Visibility.Visible)
            {
                PlayPause_Click(null, null);
            }
        }

        private void MovieGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var movie = e.ClickedItem as Movie;
            if (movie != null)
            {
                ShowMovieDetails(movie);
            }
        }

        private void ShowMovieDetails(Movie movie)
        {
            _currentMovie = movie;

            // Set basic info
            DetailTitle.Text = movie.Title;
            DetailYear.Text = !string.IsNullOrEmpty(movie.ReleaseDate) ? movie.ReleaseDate.Split('-')[0] : movie.Year.ToString();
            DetailRuntime.Text = movie.Runtime > 0 ? $"{movie.Runtime} min" : "Runtime unknown";
            DetailRating.Text = movie.VoteAverage > 0 ? $"⭐ {movie.VoteAverage:F1}/10" : "";
            DetailGenre.Text = movie.Genre;
            DetailDescription.Text = movie.Description;
            DetailCast.Text = !string.IsNullOrEmpty(movie.Cast) ? movie.Cast : "Cast information not available";

            // Set backdrop image if available
            if (!string.IsNullOrEmpty(movie.BackdropPath) && !movie.BackdropPath.Contains("ms-appx"))
            {
                DetailBackdrop.Source = new BitmapImage(new Uri(movie.BackdropPath));
            }

            DetailsOverlay.Visibility = Visibility.Visible;
        }

        private void CloseDetails_Click(object? sender, RoutedEventArgs? e)
        {
            DetailsOverlay.Visibility = Visibility.Collapsed;
            _currentMovie = null;
        }

        private void PlayMovie_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMovie == null) return;

            DetailsOverlay.Visibility = Visibility.Collapsed;
            PlayerOverlay.Visibility = Visibility.Visible;

            // Get the main window handle
            var mainWindow = App.Services.GetRequiredService<MainWindow>();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);

            // Set the video output to the window
            _mediaPlayer!.Hwnd = hwnd;

            // Play movie
            var media = new Media(_libVLC, _currentMovie.FilePath, FromType.FromPath);
            _mediaPlayer.Play(media);
            PlayPauseButton.Content = "⏸";

            // Setup time updates
            _mediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
        }

        private void MediaPlayer_TimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_mediaPlayer == null) return;

                var current = TimeSpan.FromMilliseconds(_mediaPlayer.Time);
                var total = TimeSpan.FromMilliseconds(_mediaPlayer.Length);

                TimeDisplay.Text = $"{current:mm\\:ss} / {total:mm\\:ss}";

                if (_mediaPlayer.Length > 0)
                {
                    ProgressSlider.Value = (_mediaPlayer.Time / (double)_mediaPlayer.Length) * 100;
                }
            });
        }

        private void PlayPause_Click(object? sender, RoutedEventArgs? e)
        {
            if (_mediaPlayer == null) return;

            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Pause();
                PlayPauseButton.Content = "▶";
            }
            else
            {
                _mediaPlayer.Play();
                PlayPauseButton.Content = "⏸";
            }
        }

        private void SkipForward_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Time += 10000; // 10 seconds
            }
        }

        private void SkipBackward_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Time -= 10000; // 10 seconds
            }
        }

        private void ProgressSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (_mediaPlayer != null && _mediaPlayer.Length > 0)
            {
                _mediaPlayer.Position = (float)(e.NewValue / 100.0);
            }
        }

        private void ClosePlayer_Click(object? sender, RoutedEventArgs? e)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.TimeChanged -= MediaPlayer_TimeChanged;
                _mediaPlayer.Stop();
                _mediaPlayer.Hwnd = IntPtr.Zero; // Release the window handle
            }

            PlayerOverlay.Visibility = Visibility.Collapsed;
            VideoContainer.Child = null;
            _currentMovie = null;
        }
    }
}