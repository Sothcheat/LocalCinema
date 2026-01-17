using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using LocalCinema.Models;

namespace LocalCinema.ViewModels
{
    public partial class VideoPlayerViewModel : ObservableObject
    {
        [ObservableProperty]
        private Movie? _currentMovie;

        [ObservableProperty]
        private bool _isPlaying;

        [ObservableProperty]
        private bool _isVisible;

        [ObservableProperty]
        private TimeSpan _currentTime;

        [ObservableProperty]
        private TimeSpan _duration;

        [ObservableProperty]
        private double _progress;

        public MediaPlayer? MediaPlayer { get; set; }

        public void PlayMovie(Movie movie)
        {
            CurrentMovie = movie;
            IsVisible = true;
        }

        [RelayCommand]
        private void TogglePlayPause()
        {
            if (MediaPlayer == null) return;

            if (IsPlaying)
                MediaPlayer.Pause();
            else
                MediaPlayer.Play();

            IsPlaying = !IsPlaying;
        }

        [RelayCommand]
        private void SkipForward()
        {
            if (MediaPlayer == null) return;
            MediaPlayer.Time += 10000; // 10 seconds
        }

        [RelayCommand]
        private void SkipBackward()
        {
            if (MediaPlayer == null) return;
            MediaPlayer.Time -= 10000; // 10 seconds
        }

        [RelayCommand]
        private void Close()
        {
            MediaPlayer?.Stop();
            IsVisible = false;
            CurrentMovie = null;
        }

        public void Seek(double position)
        {
            if (MediaPlayer == null) return;
            MediaPlayer.Position = (float)position;
        }
    }
}