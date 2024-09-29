using System;
using System.IO;
using WindowTextExtractor.Utils;
using Newtonsoft.Json;

namespace WindowTextExtractor.Settings
{
    public static class ApplicationSettingsFile
    {
        private const string ImageFileName = "TargetIcon";
        private const string SettingsFileName = "Settings.json";

        public static ApplicationSettings Read()
        {
            var fileInfo = GetCurrentDirectoryFileInfo(SettingsFileName);
            if (fileInfo.Exists)
            {
                if (fileInfo.Length == 0)
                {
                    return ApplicationSettings.CreateDefault();
                }
                return Read(fileInfo.FullName);
            }

            fileInfo = GetProfileFileInfo(SettingsFileName);
            if (!fileInfo.Exists)
            {
                return ApplicationSettings.CreateDefault();
            }
            return Read(fileInfo.FullName);
        }

        public static void Save(ApplicationSettings settings)
        {
            var fileInfo = GetCurrentDirectoryFileInfo(SettingsFileName);
            if (fileInfo.Exists)
            {
                Save(fileInfo, settings);
                return;
            }

            fileInfo = GetProfileFileInfo(SettingsFileName);
            Save(fileInfo, settings);
        }

        public static FileInfo GetImageFileName()
        {
            var fileInfo = GetCurrentDirectoryFileInfo(SettingsFileName);
            if (fileInfo.Exists)
            {
                return GetCurrentDirectoryFileInfo(ImageFileName);
            }

            return GetProfileFileInfo(ImageFileName);
        }

        private static FileInfo GetCurrentDirectoryFileInfo(string fileName)
        {
            var fullFileName = Path.Combine(AssemblyUtils.AssemblyDirectory, fileName);
            return new FileInfo(fullFileName);
        }

        private static FileInfo GetProfileFileInfo(string fileName)
        {
            var directoryName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AssemblyUtils.AssemblyTitle, AssemblyUtils.AssemblyProductVersion);
            var fullFileName = Path.Combine(directoryName, fileName);
            return new FileInfo(fullFileName);
        }

        private static ApplicationSettings Read(string fileName)
        {
            var jsonContent = File.ReadAllText(fileName);
            var settings = JsonConvert.DeserializeObject<ApplicationSettings>(jsonContent);
            settings.Font ??= new FontSettings();
            settings.Magnifier ??= new MagnifierSettings();
            return settings;
        }

        private static void Save(FileInfo fileInfo, ApplicationSettings settings)
        {
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(fileInfo.FullName, json);
        }
    }
}