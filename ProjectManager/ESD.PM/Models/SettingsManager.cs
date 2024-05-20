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
            return JsonConvert.DeserializeObject<AppSettings>(json);
        }

        public static void SaveSettings(AppSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsFilePath, json);
        }
    }
}
