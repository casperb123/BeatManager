using BeatManager.Entities;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            _ = GetBeatSaberPath(Settings.CurrentSettings.BeatSaberCopy, false).ConfigureAwait(false);
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

        public async Task GetBeatSaberPath(bool copy, bool toggleChanged, bool forceChangePath = false)
        {
            if (!string.IsNullOrWhiteSpace(Settings.CurrentSettings.RootPath) && Directory.Exists(Settings.CurrentSettings.RootPath) && !forceChangePath)
                return;

            BeatSaberPath = null;

            if (copy)
                await GetCopyBeatSaber();
            else
                await GetOriginalBeatSaber();

            if (string.IsNullOrWhiteSpace(Settings.CurrentSettings.RootPath) && string.IsNullOrEmpty(BeatSaberPath))
            {
                if (copy)
                    await GetOriginalBeatSaber();
                else
                    await GetCopyBeatSaber();

                if (!string.IsNullOrEmpty(BeatSaberPath))
                {
                    ChangePath = false;
                    if (copy)
                        Settings.CurrentSettings.BeatSaberCopy = false;
                    else
                        Settings.CurrentSettings.BeatSaberCopy = true;
                }
            }

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
                else if (toggleChanged)
                {
                    ChangePath = false;
                    Settings.CurrentSettings.BeatSaberCopy = !copy;
                }
            }
            else
            {
                MessageDialogResult result = MessageDialogResult.Canceled;

                await Application.Current.Dispatcher.Invoke(async () =>
                {
                    result = await MainWindow.ShowMessageAsync("Beat Saber folder found", "The following folder was found, would you like to use it?\n" +
                                                                                          $"'{BeatSaberPath}'", MessageDialogStyle.AffirmativeAndNegative);
                });

                if (result == MessageDialogResult.Affirmative)
                {
                    Settings.CurrentSettings.RootPath = BeatSaberPath;
                    Settings.CurrentSettings.Save();
                    Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
                    App.GetSupportedMods();
                    SongsPathChanged = true;
                }
                else
                {
                    if (BrowsePath())
                    {
                        Settings.CurrentSettings.RootPath = BeatSaberPath;
                        Settings.CurrentSettings.Save();
                        Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
                        App.GetSupportedMods();
                        SongsPathChanged = true;
                    }
                    else if (toggleChanged)
                    {
                        ChangePath = false;
                        Settings.CurrentSettings.BeatSaberCopy = !copy;
                    }
                }
            }
        }

        private async Task GetCopyBeatSaber()
        {
            DirectoryInfo documents = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            await FindBeatSaber(documents);
        }

        private async Task GetOriginalBeatSaber()
        {
            DirectoryInfo programFiles = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            DirectoryInfo programFiles86 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            string mainDriveLetter = Path.GetPathRoot(programFiles.FullName);

            await FindBeatSaber(programFiles);
            if (string.IsNullOrEmpty(BeatSaberPath))
                await FindBeatSaber(programFiles86);
            if (string.IsNullOrEmpty(BeatSaberPath))
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
                        await FindBeatSaber(new DirectoryInfo(steamPath));
                }
            }
        }

        private async Task FindBeatSaber(DirectoryInfo root)
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
                    if ((dirInfo.Name == "Beat Saber_Data" || File.Exists($@"{dirInfo.FullName}\Beat Saber.exe")) &&
                        dirInfo.Parent.Name.Contains("Beat") && dirInfo.Parent.Name.Contains("Saber"))
                    {
                        BeatSaberPath = dirInfo.Parent.FullName;
                        state.Break();
                    }
                    else
                        _ = FindBeatSaber(dirInfo).ConfigureAwait(false);
                });
            }
        }

        public void ChangeTheme(string theme, string color)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{color}");
        }
    }
}
