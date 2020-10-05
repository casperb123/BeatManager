using BeatManager.Entities;
using BeatSaverApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace BeatManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static BeatSaver BeatSaverApi;
        public static List<SupportedMod> SupportedMods { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            string appGuid = "a5d7dcc7-73fc-41c5-805b-b2fe96f7abdc";
            string mutexId = $@"Global\\{appGuid}";

            using (Mutex mutex = new Mutex(false, mutexId))
            {
                try
                {
                    if (!mutex.WaitOne(0, false))
                    {

                    }
                }
                finally
                {

                }
            }

            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("Only one instance of this application is allowed", "Multiple instances", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }

            SupportedMods = new List<SupportedMod>();

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
            SupportedMods = new List<SupportedMod>();

            if (Directory.Exists(Settings.CurrentSettings.PluginsPath))
            {
                List<string> modFiles = Directory.GetFiles(Settings.CurrentSettings.PluginsPath).ToList();
                modFiles.ForEach(x => SupportedMods.Add(new SupportedMod(Path.GetFileNameWithoutExtension(x).Replace(" ", ""), 2)));
            }
            else if (File.Exists(Settings.CurrentSettings.ModSupportPath))
            {
                List<SupportedMod> supportedMods = null;

                try
                {
                    string json = File.ReadAllText(Settings.CurrentSettings.ModSupportPath);
                    supportedMods = JsonConvert.DeserializeObject<List<SupportedMod>>(json);
                }
                catch (Exception) { }

                if (supportedMods != null)
                    supportedMods.ForEach(x => SupportedMods.Add(new SupportedMod(x.Name.Replace(" ", ""), x.Supported)));
            }
        }
    }
}
