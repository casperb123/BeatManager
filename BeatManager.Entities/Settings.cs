using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace BeatManager.Entities
{
    public class Settings : INotifyPropertyChanged
    {
        private string rootPath;
        private int theme;
        private int color;
        private bool beatSaberCopy;
        private bool checkForUpdates;
        private bool beatSaverOneClickInstaller;

        public static Settings CurrentSettings;
        public static string SettingsFilePath;

        public bool BeatSaverOneClickInstaller
        {
            get { return beatSaverOneClickInstaller; }
            set
            {
                beatSaverOneClickInstaller = value;
                OnPropertyChanged(nameof(BeatSaverOneClickInstaller));
            }
        }

        public bool BeatSaberCopy
        {
            get { return beatSaberCopy; }
            set
            {
                beatSaberCopy = value;
                OnPropertyChanged(nameof(BeatSaberCopy));
            }
        }

        public int Color
        {
            get { return color; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Color), "The color can't be lower than 0");

                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        public int Theme
        {
            get { return theme; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Color), "The theme can't be lower than 0");

                theme = value;
                OnPropertyChanged(nameof(Theme));
            }
        }

        public string RootPath
        {
            get { return rootPath; }
            set
            {
                rootPath = value;
                OnPropertyChanged(nameof(RootPath));

                if (!string.IsNullOrWhiteSpace(value) && Directory.Exists(value))
                {
                    string dataPath = $@"{value}\Beat Saber_Data";
                    string customLevelsPath = $@"{dataPath}\CustomLevels";

                    if (!Directory.Exists(dataPath))
                        Directory.CreateDirectory(dataPath);
                    if (!Directory.Exists(customLevelsPath))
                        Directory.CreateDirectory(customLevelsPath);
                }
            }
        }

        [JsonIgnore]
        public string CustomLevelsPath
        {
            get { return $@"{RootPath}\Beat Saber_Data\CustomLevels"; }
        }

        [JsonIgnore]
        public string PluginsPath
        {
            get { return $@"{RootPath}\Plugins"; }
        }

        [JsonIgnore]
        public string ModSupportPath
        {
            get { return $@"{RootPath}\ModSupport.json"; }
        }

        public bool CheckForUpdates
        {
            get { return checkForUpdates; }
            set
            {
                checkForUpdates = value;
                OnPropertyChanged(nameof(CheckForUpdates));
            }
        }

        public bool NotifyUpdates { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public Settings()
        {
            Color = 1;
            BeatSaberCopy = true;
            CheckForUpdates = true;
            NotifyUpdates = true;
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }

        public static Settings GetSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                Settings newSettings = new Settings();
                newSettings.Save();
            }

            JObject obj = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(SettingsFilePath));
            List<JProperty> removedProperties = obj.Properties().Where(x => typeof(Settings).GetProperty(x.Name) is null).ToList();

            if (removedProperties != null && removedProperties.Count > 0)
                removedProperties.ForEach(x => obj.Remove(x.Name));

            bool missingProperties = typeof(Settings).GetProperties().Any(x => obj.Property(x.Name) is null);
            Settings settings = obj.ToObject<Settings>();

            if (missingProperties)
                settings.Save();

            return settings;
        }
    }
}
