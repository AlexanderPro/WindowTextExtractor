using System;
using System.IO;
using Newtonsoft.Json;

namespace WindowTextExtractor.Settings
{
    public static class ApplicationSettingsFile
    {
        private const string SettingsFileName = "Settings.json";
        private const string SettingsDirectory = "WindowTextExtractor";

        public static void Save(ApplicationSettings settings, string fileName = SettingsFileName)
        {
            var fileInfo = GetFileInfo(fileName);
            if (!Directory.Exists(fileInfo.Directory.FullName))
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(fileInfo.FullName, json);
        }

        public static ApplicationSettings Load(string fileName = SettingsFileName)
        {
            var fileInfo = GetFileInfo(fileName);
            if (!File.Exists(fileInfo.FullName))
            {
                return ApplicationSettings.CreateDefault();
            }

            var jsonContent = File.ReadAllText(fileInfo.FullName);
            return JsonConvert.DeserializeObject<ApplicationSettings>(jsonContent);
        }

        private static FileInfo GetFileInfo(string fileName) => new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SettingsDirectory, fileName));
    }
}
