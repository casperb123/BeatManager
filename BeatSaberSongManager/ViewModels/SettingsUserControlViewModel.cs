using BeatSaberSongManager.Entities;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace BeatSaberSongManager.ViewModels
{
    public class SettingsUserControlViewModel
    {
        private MainWindow mainWindow;
        private readonly string copyPath;
        private readonly string[] originalPaths;

        public bool SongsPathChanged = false;

        public SettingsUserControlViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFiles86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            copyPath = $@"{documentsPath}\BeatSaber\Beat Saber_Data\CustomLevels";
            originalPaths = new string[]
            {
                $@"{programFilesPath}\Steam\steamapps\common\Beat Saber\Beat Saber_Data\CustomLevels",
                $@"{programFiles86Path}\Steam\steamapps\common\Beat Saber\Beat Saber_Data\CustomLevels",
                @"D:\Steam\steamapps\common\Beat Saber\Beat Saber_Data\CustomLevels"
            };

            DetectPath(Settings.CurrentSettings.BeatSaberCopy);
        }

        public void BrowsePath(bool saveSettings = false)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            {
                UseDescriptionForTitle = true,
                Description = "Select a folder for the songs"
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                Settings.CurrentSettings.SongsPath = dialog.SelectedPath;
                SongsPathChanged = true;
            }
        }

        public void DetectPath(bool copy)
        {
            bool songsPathNull = Settings.CurrentSettings.SongsPath == null;

            if (copy)
            {
                if (Directory.Exists(copyPath))
                {
                    Settings.CurrentSettings.SongsPath = copyPath;
                    SongsPathChanged = true;
                    if (songsPathNull)
                        Settings.CurrentSettings.Save();
                }
                else
                    BrowsePath(songsPathNull);
            }
            else
            {
                string originalPath = null;
                foreach (string path in originalPaths)
                {
                    if (Directory.Exists(path))
                    {
                        originalPath = path;
                        break;
                    }
                }

                if (originalPath != null)
                {
                    Settings.CurrentSettings.SongsPath = originalPath;
                    SongsPathChanged = true;
                    if (songsPathNull)
                        Settings.CurrentSettings.Save();
                }
                else
                    BrowsePath(songsPathNull);
            }
        }

        public void ChangeTheme(string theme, string color)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{color}");
        }
    }
}
