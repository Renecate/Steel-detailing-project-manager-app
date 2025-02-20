using ESD.PM.Settings;
using Newtonsoft.Json;
using System.IO;
using System.Net.NetworkInformation;

namespace ESD.PM.Models
{
    public static class ServerSettingsManager
    {
        private static readonly string remoteDirectoryPath = @"\\192.168.10.2\esddatabase";
        private static readonly string localDirectoryPath = @"C:\ESD.PM\offline";
        private static readonly string settingsFileName = "sharedsettings.json";
        private static string settingsDirectoryPath => IsServerAccessible() ? remoteDirectoryPath : localDirectoryPath;
        private static string settingsFilePath => Path.Combine(settingsDirectoryPath, settingsFileName);
        private static bool IsServerAccessible()
        {
            try
            {
                return Directory.Exists(@"\\192.168.10.2\esddatabase");
            }
            catch
            {
                return false;
            }
        }

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
