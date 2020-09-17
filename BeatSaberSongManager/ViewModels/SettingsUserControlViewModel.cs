using BeatSaberSongManager.Entities;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
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
        private readonly string copyPath;
        private readonly string[] originalPaths;

        public bool SongsPathChanged = false;

        public SettingsUserControlViewModel()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFiles86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            copyPath = $@"{documentsPath}\BeatSaber";
            originalPaths = new string[]
            {
                $@"{programFilesPath}\Steam\steamapps\common\Beat Saber",
                $@"{programFiles86Path}\Steam\steamapps\common\Beat Saber",
                @"D:\Steam\steamapps\common\Beat Saber"
            };

            DetectPath(Settings.CurrentSettings.BeatSaberCopy);
        }

        public void BrowsePath()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog
            {
                UseDescriptionForTitle = true,
                Description = "Select a folder for the beatmaps"
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                Settings.CurrentSettings.SongsPath = dialog.SelectedPath;
                App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath;
                SongsPathChanged = true;
            }
        }

        public void DetectPath(bool copy, bool forceChangePath = false)
        {
            bool songsPathNull = Settings.CurrentSettings.SongsPath == null;
            if (!songsPathNull && !forceChangePath)
                return;

            if (copy)
            {
                if (Directory.Exists(copyPath))
                {
                    Settings.CurrentSettings.SongsPath = copyPath;
                    App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath;
                    SongsPathChanged = true;
                    if (songsPathNull)
                        Settings.CurrentSettings.Save();
                }
                else
                    BrowsePath();
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
                    App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath;
                    SongsPathChanged = true;
                    if (songsPathNull)
                        Settings.CurrentSettings.Save();
                }
                else
                    BrowsePath();
            }
        }

        public void ChangeTheme(string theme, string color)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{color}");
        }
    }
}
