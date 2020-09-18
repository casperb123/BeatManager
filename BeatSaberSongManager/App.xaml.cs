using BeatSaberSongManager.Entities;
using BeatSaverApi;
using System;
using System.IO;
using System.Windows;

namespace BeatSaberSongManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static BeatSaver BeatSaverApi;

        protected override void OnStartup(StartupEventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsPath = $@"{appDataPath}\BeatSaberSongManager";

            Settings.SettingsFilePath = $@"{settingsPath}\Settings.json";

            if (!Directory.Exists(settingsPath))
                Directory.CreateDirectory(settingsPath);

            Settings.CurrentSettings = Settings.GetSettings();
            BeatSaverApi = new BeatSaver(Settings.CurrentSettings.CustomLevelsPath);
            base.OnStartup(e);
        }
    }
}
