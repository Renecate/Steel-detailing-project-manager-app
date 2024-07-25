using Newtonsoft.Json;
using System.IO;

namespace ESD.PM.Models
{
    public static class SettingsManager
    {
        private static readonly string settingsFilePath = "appsettings.json";

        public static AppSettings LoadSettings()
        {
            if (!File.Exists(settingsFilePath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(settingsFilePath);

            var settings = JsonConvert.DeserializeObject<AppSettings>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });

            return settings ?? new AppSettings();
        }

        public static void SaveSettings(AppSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(settingsFilePath, json);
        }
    }
}
