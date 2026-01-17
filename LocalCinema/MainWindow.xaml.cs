using Microsoft.UI.Xaml;
using LocalCinema.Views;
using System;

namespace LocalCinema
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            Title = "LocalCinema";

            // Set window size
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1400, 900));

            // Navigate to library page
            this.Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            // Only navigate once
            if (args.WindowActivationState != WindowActivationState.Deactivated)
            {
                this.Activated -= MainWindow_Activated;

                try
                {
                    RootFrame.Navigate(typeof(LibraryPage));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                }
            }
        }
    }
}