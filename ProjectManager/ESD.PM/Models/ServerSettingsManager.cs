using Newtonsoft.Json;
using System.IO;

namespace ESD.PM.Models
{
    public static class ServerSettingsManager
    {
        private static readonly string settingsDirectoryPath = @"\\192.168.10.2\esddatabase";
        private static readonly string settingsFilePath = Path.Combine(settingsDirectoryPath, "sharedsettings.json");
        public static SharedSettings LoadSettings()
        {
            if (!File.Exists(settingsFilePath))
            {
                return new SharedSettings();
            }

            var json = File.ReadAllText(settingsFilePath);

            var settings = JsonConvert.DeserializeObject<SharedSettings>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });

            return settings ?? new SharedSettings();
        }

        public static void SaveSettings(SharedSettings settings)
        {
            if (!Directory.Exists(settingsDirectoryPath))
            {
                Directory.CreateDirectory(settingsDirectoryPath);
            }

            var json = JsonConvert.SerializeObject(settings, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(settingsFilePath, json);
        }
    }
}
