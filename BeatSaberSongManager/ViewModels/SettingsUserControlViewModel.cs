using BeatSaberSongManager.Entities;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Ookii.Dialogs.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace BeatSaberSongManager.ViewModels
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
            GetBeatSaberPath(Settings.CurrentSettings.BeatSaberCopy, false).ConfigureAwait(false);
        }

        public bool BrowsePath()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog
            {
                UseDescriptionForTitle = true,
                Description = "Select the folder that has the 'Beat Saber_Data' folder inside"
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
            if (!string.IsNullOrWhiteSpace(Settings.CurrentSettings.SongsPath) && Directory.Exists(Settings.CurrentSettings.SongsPath) && !forceChangePath)
                return;

            BeatSaberPath = null;

            if (copy)
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                FindBeatSaber(new DirectoryInfo(documentsPath));
            }
            else
            {
                string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string programFiles86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                FindBeatSaber(new DirectoryInfo(programFilesPath));
                if (string.IsNullOrEmpty(BeatSaberPath))
                    FindBeatSaber(new DirectoryInfo(programFiles86Path));
            }

            if (string.IsNullOrEmpty(BeatSaberPath))
            {
                if (BrowsePath())
                {
                    Settings.CurrentSettings.SongsPath = BeatSaberPath;
                    Settings.CurrentSettings.Save();
                    Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
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
                                                                                                          $"{BeatSaberPath}", MessageDialogStyle.AffirmativeAndNegative);
                });

                if (result == MessageDialogResult.Affirmative)
                {
                    Settings.CurrentSettings.SongsPath = BeatSaberPath;
                    Settings.CurrentSettings.Save();
                    Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
                    SongsPathChanged = true;
                }
                else
                {
                    if (BrowsePath())
                    {
                        Settings.CurrentSettings.SongsPath = BeatSaberPath;
                        Settings.CurrentSettings.Save();
                        Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
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
                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    if (dirInfo.Name == "Beat Saber")
                    {
                        BeatSaberPath = dirInfo.FullName;
                        break;
                    }
                    else if (dirInfo.Name == "BeatSaber")
                    {
                        BeatSaberPath = dirInfo.FullName;
                        break;
                    }
                    else
                        FindBeatSaber(dirInfo);
                }
            }
        }

        public void ChangeTheme(string theme, string color)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{color}");
        }
    }
}
