﻿using BeatManager.Entities;
using BeatSaverApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace BeatManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static BeatSaver BeatSaverApi;
        public static List<string> SupportedMods { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("Only one instance of this application is allowed", "Multiple instances", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mainProjectName = Assembly.GetEntryAssembly().GetName().Name;
            string appDataPath = $@"{appData}\{mainProjectName}";

            Settings.SettingsFilePath = $@"{appDataPath}\Settings.json";

            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            Settings.CurrentSettings = Settings.GetSettings();
            BeatSaverApi = new BeatSaver(Settings.CurrentSettings.CustomLevelsPath);
            GetSupportedMods();

            base.OnStartup(e);
        }

        public static void GetSupportedMods()
        {
            if (Directory.Exists(Settings.CurrentSettings.PluginsPath))
            {
                SupportedMods = new List<string>();
                List<string> modFiles = Directory.GetFiles(Settings.CurrentSettings.PluginsPath).ToList();
                modFiles.ForEach(x => SupportedMods.Add(Path.GetFileNameWithoutExtension(x).Replace(" ", "")));
            }
            else if (File.Exists(Settings.CurrentSettings.ModSupportPath))
            {
                SupportedMods = new List<string>();
                string json = File.ReadAllText(Settings.CurrentSettings.ModSupportPath);
                List<string> supportedMods = JsonConvert.DeserializeObject<List<string>>(json);
                supportedMods.ForEach(x => SupportedMods.Add(x.Replace(" ", "")));
            }
        }
    }
}