using BeatSaberSongManager.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BeatSaberSongManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsPath = $@"{appDataPath}\BeatSaberSongManager";

            Settings.SettingsFilePath = $@"{settingsPath}\Settings.json";

            if (!Directory.Exists(settingsPath))
                Directory.CreateDirectory(settingsPath);

            Settings.CurrentSettings = Settings.GetSettings();
            base.OnStartup(e);
        }
    }
}
