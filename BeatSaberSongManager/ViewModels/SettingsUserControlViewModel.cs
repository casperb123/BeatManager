using BeatSaberSongManager.Entities;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace BeatSaberSongManager.ViewModels
{
    public class SettingsUserControlViewModel
    {
        private MainWindow mainWindow;
        private readonly string defaultSongsPath;

        public SettingsUserControlViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            defaultSongsPath = $@"{documentsPath}\BeatSaber\Beat Saber_Data\CustomLevels";

            CheckSongsPath();
        }

        private async void CheckSongsPath()
        {
            if (string.IsNullOrWhiteSpace(Settings.CurrentSettings.SongsPath) ||
                !Directory.Exists(Path.GetDirectoryName(Settings.CurrentSettings.SongsPath)))
            {
                if (Directory.Exists(defaultSongsPath))
                {
                    MessageDialogResult result = await mainWindow.ShowMessageAsync("No songs path specified", "Would you like to use the default path to download songs or would you like to pick one yourself?\n\n" +
                                                                                                              "Click Ok to use the default path\n" +
                                                                                                              "Click Cancel to pick one yourself", MessageDialogStyle.AffirmativeAndNegative);

                    if (result == MessageDialogResult.Affirmative)
                    {
                        Settings.CurrentSettings.SongsPath = defaultSongsPath;
                        Settings.CurrentSettings.Save();
                    }
                    else
                        SetSongsPath(true, true);
                }
                else
                    SetSongsPath(false, true);
            }
        }

        public void SetSongsPath(bool useDefault, bool saveSettings)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            {
                UseDescriptionForTitle = true,
                Description = "Select a folder for the songs"
            };

            if (dialog.ShowDialog().GetValueOrDefault())
                Settings.CurrentSettings.SongsPath = dialog.SelectedPath;
            else if (useDefault)
                Settings.CurrentSettings.SongsPath = defaultSongsPath;

            if (saveSettings)
                Settings.CurrentSettings.Save();
        }

        public void ChangeTheme(string theme, string color)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{color}");
        }
    }
}
