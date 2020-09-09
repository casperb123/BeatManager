using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatSaberSongManager.Entities
{
    public class Settings : INotifyPropertyChanged
    {
        private string songsPath;
        private int theme;
        private int color;

        public static Settings CurrentSettings;
        public static string SettingsFilePath;

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

        public string SongsPath
        {
            get { return songsPath; }
            set
            {
                songsPath = value;
                OnPropertyChanged(nameof(SongsPath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Settings()
        {
            Theme = 1;
        }

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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
