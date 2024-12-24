using ESD.PM.Settings;
using Newtonsoft.Json;
using System.IO;

namespace ESD.PM.Models
{
    public static class FoldersSettingsManager
    {
        private static readonly string settingsDirectoryPath = @"C:\ESD.PM";
        private static readonly string settingsFilePath = Path.Combine(settingsDirectoryPath, "folderssettings.json");

        public static FoldersSettings LoadSettings()
        {
            if (!File.Exists(settingsFilePath))
            {
                return new FoldersSettings();
            }

            var json = File.ReadAllText(settingsFilePath);

            var settings = JsonConvert.DeserializeObject<FoldersSettings>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });

            return settings ?? new FoldersSettings();
        }

        public static void SaveSettings(FoldersSettings settings)
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
