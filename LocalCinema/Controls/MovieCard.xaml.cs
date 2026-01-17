using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using LocalCinema.Models;

namespace LocalCinema.Controls
{
    public sealed partial class MovieCard : UserControl
    {
        public static readonly DependencyProperty MovieProperty =
            DependencyProperty.Register(
                nameof(Movie),
                typeof(Movie),
                typeof(MovieCard),
                new PropertyMetadata(null));

        public Movie Movie
        {
            get => (Movie)GetValue(MovieProperty);
            set => SetValue(MovieProperty, value);
        }

        public MovieCard()
        {
            this.InitializeComponent();
        }

        private void Card_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CardBorder.Translation = new System.Numerics.Vector3(0, -8, 32);
        }

        private void Card_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CardBorder.Translation = new System.Numerics.Vector3(0, 0, 0);
        }

        // Helper methods for formatting
        public string FormatRuntime(int runtime)
        {
            if (runtime <= 0) return "";
            return $"{runtime}m";
        }

        public string FormatRating(double rating)
        {
            if (rating <= 0) return "";
            return $"⭐ {rating:F1}";
        }
    }
}