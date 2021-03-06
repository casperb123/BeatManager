﻿using BeatManager.Entities;
using BeatSaver;
using ModelSaber;
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
        public static App Instance { get; private set; }
        public static BeatSaverApi BeatSaverApi { get; private set; }
        public static ModelSaberApi ModelSaberApi { get; private set; }
        public static List<SupportedMod> SupportedMods { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string mainProjectName = Assembly.GetEntryAssembly().GetName().Name;
            string appDataPath = $@"{appData}\{mainProjectName}";

            Settings.SettingsFilePath = $@"{appDataPath}\Settings.json";
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            Settings.CurrentSettings = Settings.GetSettings();

            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                string[] args = Environment.GetCommandLineArgs();
                string beatSaverArg = args.FirstOrDefault(x => x.StartsWith("beatsaver://"));
                string modelSaberArg = args.FirstOrDefault(x => x.StartsWith("modelsaber://"));

                if (!string.IsNullOrEmpty(beatSaverArg))
                {
                    if (Settings.CurrentSettings.BeatSaver.OneClickInstaller)
                    {
                        try
                        {
                            string beatSaverKey = beatSaverArg.Substring(12).Replace("/", "");
                            NamedPipe<string>.Send(NamedPipe<string>.NameTypes.BeatSaver, beatSaverKey);
                            Environment.Exit(0);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            string processFilePath = Process.GetCurrentProcess().MainModule.FileName;

                            ProcessStartInfo elevated = new ProcessStartInfo(processFilePath)
                            {
                                UseShellExecute = true,
                                Verb = "runas",
                                Arguments = beatSaverArg
                            };

                            Process.Start(elevated);
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        MessageBox.Show("OneClick installer is disabled", "BeatSaver OneClick", MessageBoxButton.OK, MessageBoxImage.Error);
                        Environment.Exit(0);
                    }
                }
                if (!string.IsNullOrEmpty(modelSaberArg))
                {
                    if (Settings.CurrentSettings.ModelSaber.OneClickInstaller)
                    {
                        try
                        {
                            string modelSaberArgs = modelSaberArg.Substring(13);
                            NamedPipe<string>.Send(NamedPipe<string>.NameTypes.ModelSaber, modelSaberArgs);
                            Environment.Exit(0);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            string processFilePath = Process.GetCurrentProcess().MainModule.FileName;

                            ProcessStartInfo elevated = new ProcessStartInfo(processFilePath)
                            {
                                UseShellExecute = true,
                                Verb = "runas",
                                Arguments = modelSaberArg
                            };

                            Process.Start(elevated);
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        MessageBox.Show("OneClick installer is disabled", "ModelSaber OneClick", MessageBoxButton.OK, MessageBoxImage.Error);
                        Environment.Exit(0);
                    }
                }
            }

            Instance = this;
            SupportedMods = new List<SupportedMod>();

            BeatSaverApi = new BeatSaverApi(Settings.CurrentSettings.CustomLevelsPath);
            ModelSaberApi = new ModelSaberApi(Settings.CurrentSettings.RootPath);
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
