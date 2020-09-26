using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

namespace BeatSaberSongManager.ViewModels
{
    public class SettingsUserControlViewModel : INotifyPropertyChanged
    {
        private BeatSaber beatSaber;

        public readonly MainWindow MainWindow;
        public bool SongsPathChanged = false;
        public bool ChangePath = true;

        public BeatSaber BeatSaber
        {
            get { return beatSaber; }
            set
            {
                beatSaber = value;
                OnPropertyChanged(nameof(BeatSaber));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

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
                Description = "Select a folder for the beatmaps"
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                BeatSaber = new BeatSaber(dialog.SelectedPath, Settings.CurrentSettings.BeatSaberCopy);
                return true;
            }

            return false;
        }

        public async Task GetBeatSaberPath(bool copy, bool toggleChanged, bool forceChangePath = false)
        {
            if (!string.IsNullOrWhiteSpace(Settings.CurrentSettings.SongsPath) && Directory.Exists(Settings.CurrentSettings.SongsPath) && !forceChangePath)
                return;

            BeatSaber = null;

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
                if (BeatSaber is null)
                    FindBeatSaber(new DirectoryInfo(programFiles86Path));
            }

            if (BeatSaber is null)
            {
                if (BrowsePath())
                {
                    Settings.CurrentSettings.SongsPath = BeatSaber.Path;
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
                                                                                                          $"{BeatSaber.Path}", MessageDialogStyle.AffirmativeAndNegative);
                });

                if (result == MessageDialogResult.Affirmative)
                {
                    Settings.CurrentSettings.SongsPath = BeatSaber.Path;
                    Settings.CurrentSettings.Save();
                    Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
                    SongsPathChanged = true;
                }
                else
                {
                    if (BrowsePath())
                    {
                        Settings.CurrentSettings.SongsPath = BeatSaber.Path;
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
                        BeatSaber = new BeatSaber(dirInfo.FullName, true);
                        break;
                    }
                    else if (dirInfo.Name == "BeatSaber")
                    {
                        BeatSaber = new BeatSaber(dirInfo.FullName, false);
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
