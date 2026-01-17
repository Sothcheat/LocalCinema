using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using LocalCinema.Services;
using LocalCinema.ViewModels;
using LocalCinema.Helpers;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Diagnostics;

namespace LocalCinema
{
    public partial class App : Application
    {
        public static ServiceProvider? Services { get; private set; }
        public static Window? m_window { get; private set; }

        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += App_UnhandledException;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            try
            {
                // Initialize LibVLC
                LibVLCSharp.Shared.Core.Initialize();

                // Setup Dependency Injection
                var services = new ServiceCollection();
                ConfigureServices(services);
                Services = services.BuildServiceProvider();

                // Initialize database
                var context = Services.GetRequiredService<MovieDbContext>();
                await context.Database.EnsureCreatedAsync();

                // Create and activate main window
                m_window = Services.GetRequiredService<MainWindow>();
                m_window.Activate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Launch error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                // Show error window
                ShowErrorWindow(ex.Message);
            }
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Load settings
            var settings = SettingsHelper.LoadSettings();

            // Database
            var dbFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LocalCinema"
            );
            Directory.CreateDirectory(dbFolder);
            var dbPath = Path.Combine(dbFolder, "movies.db");

            services.AddDbContext<MovieDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // TMDb Service
            var tmdbApiKey = settings.TmdbApiKey;
            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                tmdbApiKey = "YOUR_TMDB_API_KEY_HERE"; // REPLACE WITH YOUR KEY
            }
            services.AddSingleton<ITmdbService>(sp => new TmdbService(tmdbApiKey));

            // Services
            services.AddSingleton<IMovieRepository, MovieRepository>();
            services.AddSingleton<IMovieScanner, MovieScanner>();
            services.AddSingleton<IMetadataProvider, MetadataProvider>();
            services.AddSingleton<INetworkService, NetworkService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<LibraryViewModel>();
            services.AddTransient<MovieDetailsViewModel>();
            services.AddTransient<VideoPlayerViewModel>();

            // Windows
            services.AddTransient<MainWindow>();
        }

        private void ShowErrorWindow(string message)
        {
            try
            {
                var errorWindow = new Window
                {
                    Title = "LocalCinema - Error"
                };

                var scrollViewer = new Microsoft.UI.Xaml.Controls.ScrollViewer();
                var errorText = new Microsoft.UI.Xaml.Controls.TextBlock
                {
                    Text = $"Failed to start LocalCinema:\n\n{message}\n\nCheck Output window for details.",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(20),
                    FontSize = 14
                };

                scrollViewer.Content = errorText;
                errorWindow.Content = scrollViewer;
                errorWindow.Activate();
            }
            catch
            {
                // If we can't even show an error window, just fail silently
            }
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine($"Unhandled exception: {e.Message}");
            if (e.Exception != null)
            {
                Debug.WriteLine($"Stack trace: {e.Exception.StackTrace}");
            }
            e.Handled = true;
        }
    }
}