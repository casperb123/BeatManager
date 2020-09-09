using BeatSaberSongManager.Entities;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

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
                    //MessageDialogResult result = await mainWindow.ShowMessageAsync("No songs path specified", "Would you like to use the default path to download songs or would you like to pick one yourself?\n\n" +
                    //                                                                                          "Click Ok to use the default path\n" +
                    //                                                                                          "Click Cancel to pick one yourself", MessageDialogStyle.AffirmativeAndNegative);

                    //if (result == MessageDialogResult.Affirmative)
                    //{
                        Settings.CurrentSettings.SongsPath = defaultSongsPath;
                        Settings.CurrentSettings.Save();
                    //}
                    //else
                    //    SetSongsPath(true, true);
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

        public void ChangeTheme(string themeName, PrimaryColor primaryColor)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            IBaseTheme baseTheme = themeName == "Dark" ? Theme.Dark : Theme.Light;
            Color color = SwatchHelper.Lookup[(MaterialDesignColor)primaryColor];

            theme.SetBaseTheme(baseTheme);
            theme.SetPrimaryColor(color);

            paletteHelper.SetTheme(theme);
        }
    }
}
