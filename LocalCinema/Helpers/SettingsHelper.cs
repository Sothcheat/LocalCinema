using System;
using System.IO;
using System.Text.Json;

namespace LocalCinema.Helpers
{
    public class SettingsHelper
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LocalCinema",
            "settings.json"
        );

        public static AppSettings LoadSettings()
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            return new AppSettings();
        }

        public static void SaveSettings(AppSettings settings)
        {
            var folder = Path.GetDirectoryName(SettingsPath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder!);

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
    }

    public class AppSettings
    {
        public string[] MovieFolders { get; set; } = Array.Empty<string>();
        public bool EnableNetworkStreaming { get; set; } = false;
        public int ServerPort { get; set; } = 8080;
        public string TmdbApiKey { get; set; } = string.Empty; // ADD YOUR API KEY HERE
    }
}