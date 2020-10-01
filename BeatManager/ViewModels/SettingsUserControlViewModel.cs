using BeatManager.Entities;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BeatManager.ViewModels
{
    public class SettingsUserControlViewModel
    {
        public readonly MainWindow MainWindow;
        public bool SongsPathChanged = false;
        public bool ChangePath = true;

        public string BeatSaberPath { get; set; }

        public SettingsUserControlViewModel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            if (string.IsNullOrWhiteSpace(Settings.CurrentSettings.RootPath) || !Directory.Exists(Settings.CurrentSettings.RootPath))
                _ = GetBeatSaberPath(Settings.CurrentSettings.BeatSaberCopy, true).ConfigureAwait(false);
            else
                _ = GetBeatSaberPath(Settings.CurrentSettings.BeatSaberCopy).ConfigureAwait(false);
        }

        public bool BrowsePath()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog
            {
                UseDescriptionForTitle = true,
                Description = "Select the folder that has the 'Beat Saber_Data' folder and/or the 'Beat Saber.exe' file inside"
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                BeatSaberPath = dialog.SelectedPath;
                return true;
            }

            return false;
        }

        public async Task GetBeatSaberPath(bool copy, bool forceChangePath = false)
        {
            if (!string.IsNullOrWhiteSpace(Settings.CurrentSettings.RootPath) && Directory.Exists(Settings.CurrentSettings.RootPath) && !forceChangePath)
                return;

            BeatSaberPath = null;
            GetBeatSaberFolder(copy);

            if (string.IsNullOrEmpty(BeatSaberPath))
            {
                if (BrowsePath())
                {
                    Settings.CurrentSettings.RootPath = BeatSaberPath;
                    Settings.CurrentSettings.Save();
                    Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
                    App.GetSupportedMods();
                    SongsPathChanged = true;
                }
            }
            else
            {
                bool fileExists = File.Exists($@"{BeatSaberPath}\Beat Saber.exe");
                bool useFolder = true;
                if (fileExists && Settings.CurrentSettings.BeatSaberCopy)
                {
                    MessageDialogResult result = MessageDialogResult.Canceled;
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        result = await MainWindow.ShowMessageAsync("Beat Saber folder found", "The following folder was found.\n" +
                                                                                              "Using this folder will change the version to original. Would you like to use it?\n\n" +
                                                                                              $"'{BeatSaberPath}'", MessageDialogStyle.AffirmativeAndNegative);
                    });

                    if (result != MessageDialogResult.Affirmative)
                        useFolder = false;
                }
                else if (!fileExists && !Settings.CurrentSettings.BeatSaberCopy)
                {
                    MessageDialogResult result = MessageDialogResult.Canceled;
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        result = await MainWindow.ShowMessageAsync("Beat Saber folder found", "The following folder was found.\n" +
                                                                                              "Using this folder will change the version to copy. Would you like to use it?\n\n" +
                                                                                              $"'{BeatSaberPath}'", MessageDialogStyle.AffirmativeAndNegative);
                    });

                    if (result != MessageDialogResult.Affirmative)
                        useFolder = false;
                }
                else
                {
                    MessageDialogResult result = MessageDialogResult.Canceled;
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        result = await MainWindow.ShowMessageAsync("Beat Saber folder found", "The following folder was found, would you like to use it?\n" +
                                                                                              $"'{BeatSaberPath}'", MessageDialogStyle.AffirmativeAndNegative);
                    });

                    if (result != MessageDialogResult.Affirmative)
                        useFolder = false;
                }

                if (useFolder)
                {
                    Settings.CurrentSettings.RootPath = BeatSaberPath;
                    Settings.CurrentSettings.Save();
                    Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
                    App.GetSupportedMods();
                    SongsPathChanged = true;
                }
                else
                {
                    if (BrowsePath() || (forceChangePath && !string.IsNullOrWhiteSpace(BeatSaberPath) && Directory.Exists(BeatSaberPath)))
                    {
                        Settings.CurrentSettings.RootPath = BeatSaberPath;
                        Settings.CurrentSettings.Save();
                        Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
                        App.GetSupportedMods();
                        SongsPathChanged = true;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(BeatSaberPath))
            {
                bool applicationExists = File.Exists($@"{BeatSaberPath}\Beat Saber.exe");
                if (applicationExists && Settings.CurrentSettings.BeatSaberCopy)
                {
                    ChangePath = false;
                    Settings.CurrentSettings.BeatSaberCopy = false;
                }
                else if (!applicationExists && !Settings.CurrentSettings.BeatSaberCopy)
                {
                    ChangePath = false;
                    Settings.CurrentSettings.BeatSaberCopy = true;
                }
            }
        }

        private void GetBeatSaberFolder(bool copy)
        {
            DirectoryInfo documents = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            DirectoryInfo programFiles = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            DirectoryInfo programFiles86 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

            if (copy)
            {
                FindBeatSaber(documents);

                if (BeatSaberPath is null)
                {
                    FindBeatSaber(programFiles);
                    if (BeatSaberPath is null)
                        FindBeatSaber(programFiles86);
                }
            }
            else
            {
                FindBeatSaber(programFiles);
                if (BeatSaberPath is null)
                    FindBeatSaber(programFiles86);
                if (BeatSaberPath is null)
                    FindBeatSaber(documents);
            }

            if (BeatSaberPath is null)
                GetBeatSaberPathInOtherDrives(Path.GetPathRoot(programFiles.FullName));
        }

        private void GetBeatSaberPathInOtherDrives(string mainDriveLetter)
        {
            List<DriveInfo> drives = DriveInfo.GetDrives().Where(x => x.Name != mainDriveLetter && x.DriveType == DriveType.Fixed).ToList();
            foreach (DriveInfo drive in drives)
            {
                string steamPath = null;
                if (Directory.Exists($@"{drive.RootDirectory}\Steam"))
                    steamPath = $@"{drive.RootDirectory}\Steam";
                else if (Directory.Exists($@"{drive.RootDirectory}\SteamLibrary"))
                    steamPath = $@"{drive.RootDirectory}\SteamLibrary";

                if (!string.IsNullOrEmpty(steamPath))
                    FindBeatSaber(new DirectoryInfo(steamPath));
            }
        }

        private void FindBeatSaber(DirectoryInfo root)
        {
            DirectoryInfo[] subDirs = null;

            try
            {
                subDirs = root.GetDirectories();
            }
            catch (UnauthorizedAccessException) { }

            if (subDirs != null)
            {
                Parallel.ForEach(subDirs, (dirInfo, state) =>
                {
                    if (dirInfo.Name == "Beat Saber_Data")
                    {
                        BeatSaberPath = dirInfo.Parent.FullName;
                        state.Break();
                    }
                    else if (File.Exists($@"{dirInfo.FullName}\Beat Saber.exe"))
                    {
                        BeatSaberPath = dirInfo.FullName;
                        state.Break();
                    }
                    else
                        FindBeatSaber(dirInfo);
                });
            }
        }

        public void ChangeTheme(string theme, string color)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{color}");
        }
    }
}
