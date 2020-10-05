using BeatManager.Entities;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace BeatManager.ViewModels
{
    public class SettingsUserControlViewModel : INotifyPropertyChanged
    {
        private bool isRunningAsAdmin;
        private bool isBeatSaverOneClick;

        public readonly MainWindow MainWindow;
        public bool SongsPathChanged = false;
        public bool ChangePath = true;
        public bool ChangeBeatSaverOneClick = true;

        public bool IsBeatSaverOneClick
        {
            get { return isBeatSaverOneClick; }
            set
            {
                isBeatSaverOneClick = value;
                OnPropertyChanged(nameof(IsBeatSaverOneClick));
            }
        }

        public bool IsRunningAsAdmin
        {
            get { return isRunningAsAdmin; }
            set
            {
                isRunningAsAdmin = value;
                OnPropertyChanged(nameof(IsRunningAsAdmin));
            }
        }

        public string BeatSaberPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public SettingsUserControlViewModel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;

            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            IsRunningAsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (string.IsNullOrWhiteSpace(Settings.CurrentSettings.RootPath) || !Directory.Exists(Settings.CurrentSettings.RootPath))
                _ = GetBeatSaberPath(Settings.CurrentSettings.BeatSaberCopy, true, true).ConfigureAwait(false);
            else
                _ = GetBeatSaberPath(Settings.CurrentSettings.BeatSaberCopy).ConfigureAwait(false);

            if (IsRunningAsAdmin)
            {
                var (beatSaverCallback, beatSaverprovider) = CheckOneClick(OneClickType.BeatSaver);

                if (beatSaverCallback == OneClickCallback.BeatManager)
                    IsBeatSaverOneClick = true;

                if (!IsBeatSaverOneClick && Settings.CurrentSettings.BeatSaverOneClickInstaller)
                    ToggleOneClick(OneClickType.BeatSaver, true).ConfigureAwait(false);
            }
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

        public async Task GetBeatSaberPath(bool copy, bool ignoreExists = false, bool forceChange = false)
        {
            if (!string.IsNullOrWhiteSpace(Settings.CurrentSettings.RootPath) && Directory.Exists(Settings.CurrentSettings.RootPath) && !ignoreExists)
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
                        result = await MainWindow.ShowMessageAsync("Beat Saber folder found", "The following folder was found, would you like to use it?\n\n" +
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
                    if (BrowsePath() || (forceChange && !string.IsNullOrWhiteSpace(BeatSaberPath) && Directory.Exists(BeatSaberPath)))
                    {
                        Settings.CurrentSettings.RootPath = BeatSaberPath;
                        Settings.CurrentSettings.Save();
                        Application.Current.Dispatcher.Invoke(() => App.BeatSaverApi.SongsPath = Settings.CurrentSettings.CustomLevelsPath);
                        App.GetSupportedMods();
                        SongsPathChanged = true;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(Settings.CurrentSettings.RootPath))
            {
                bool applicationExists = File.Exists($@"{Settings.CurrentSettings.RootPath}\Beat Saber.exe");
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
                    FindBeatSaber(programFiles);
                if (BeatSaberPath is null)
                    FindBeatSaber(programFiles86);
                if (BeatSaberPath is null)
                    GetBeatSaberFolderInOtherDrives(Path.GetPathRoot(programFiles.FullName));
            }
            else
            {
                FindBeatSaber(programFiles);
                if (BeatSaberPath is null)
                    FindBeatSaber(programFiles86);
                if (BeatSaberPath is null)
                    GetBeatSaberFolderInOtherDrives(Path.GetPathRoot(programFiles.FullName));
                if (BeatSaberPath is null)
                    FindBeatSaber(documents);
            }
        }

        private void GetBeatSaberFolderInOtherDrives(string mainDriveLetter)
        {
            List<DriveInfo> drives = DriveInfo.GetDrives().Where(x => x.Name != mainDriveLetter && x.DriveType == DriveType.Fixed).ToList();
            foreach (DriveInfo drive in drives)
            {
                string rootPath = null;
                if (Directory.Exists($@"{drive.RootDirectory}\Steam"))
                    rootPath = $@"{drive.RootDirectory}\Steam";
                else if (Directory.Exists($@"{drive.RootDirectory}\SteamLibrary"))
                    rootPath = $@"{drive.RootDirectory}\SteamLibrary";
                else if (Directory.Exists($@"{drive.RootDirectory}\Games"))
                    rootPath = $@"{drive.RootDirectory}\Games";

                if (!string.IsNullOrEmpty(rootPath))
                    FindBeatSaber(new DirectoryInfo(rootPath));
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

        public (OneClickCallback callback, string provider) CheckOneClick(OneClickType oneClickType)
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            string applicationPath = $@"{Directory.GetCurrentDirectory()}\{processName}.exe";

            switch (oneClickType)
            {
                case OneClickType.BeatSaver:
                    RegistryKey beatSaverKey = Registry.ClassesRoot.OpenSubKey("beatsaver");
                    if (beatSaverKey is null ||
                        beatSaverKey.GetValue("").ToString() != "URL:beatsaver" ||
                        beatSaverKey.GetValue("URL Protocol") is null)
                    {
                        return (OneClickCallback.KeyError, null);
                    }
                    RegistryKey commandKey = beatSaverKey.OpenSubKey(@"shell\open\command");
                    if (commandKey is null)
                        return (OneClickCallback.KeyError, null);

                    string[] commandKeyValues = commandKey.GetValue("").ToString().Replace("\"", "").Split(" ");
                    if (commandKeyValues.Length != 2 || commandKeyValues[1] != "%1")
                        return (OneClickCallback.KeyError, null);

                    if (beatSaverKey.GetValue("OneClick-Provider") != null)
                    {
                        string provider = beatSaverKey.GetValue("OneClick-Provider").ToString();
                        if (provider != "BeatManager")
                            return (OneClickCallback.OtherProvider, provider);
                    }
                    if (commandKeyValues[0] != applicationPath)
                    {
                        string provider = Path.GetFileNameWithoutExtension(commandKeyValues[0]);
                        if (provider != "BeatManager")
                            return (OneClickCallback.OtherProvider, provider);
                    }

                    return (OneClickCallback.BeatManager, null);
                default:
                    return (OneClickCallback.Null, null);
            }
        }

        public async Task<bool> ToggleOneClick(OneClickType oneClickType, bool enable)
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            string applicationPath = $@"{Directory.GetCurrentDirectory()}\{processName}.exe";

            if (oneClickType == OneClickType.BeatSaver)
            {
                if (enable)
                {
                    var (callback, provider) = CheckOneClick(oneClickType);

                    if (callback == OneClickCallback.OtherProvider)
                    {
                        MessageDialogResult result = await MainWindow.ShowMessageAsync($"BeatSaver OneClick", $"The OneClick provider for BeatSaver is currently {provider}. Would you like to enable it anyways?", MessageDialogStyle.AffirmativeAndNegative);
                        if (result != MessageDialogResult.Affirmative)
                            return false;
                    }

                    RegistryKey beatSaverKey = Registry.ClassesRoot.CreateSubKey("beatsaver", true);
                    beatSaverKey.SetValue("", "URL:beatsaver");
                    beatSaverKey.SetValue("URL Protocol", string.Empty);
                    beatSaverKey.SetValue("OneClick-Provider", "BeatManager");

                    RegistryKey commandKey = beatSaverKey.CreateSubKey(@"shell\open\command", true);
                    commandKey.SetValue("", $"\"{applicationPath}\" \"%1\"");

                    IsBeatSaverOneClick = true;
                }
                else
                {
                    RegistryKey beatSaverKey = Registry.ClassesRoot.OpenSubKey("beatsaver");
                    if (beatSaverKey != null)
                        Registry.ClassesRoot.DeleteSubKeyTree("beatsaver");

                    IsBeatSaverOneClick = false;
                }
            }

            return true;
        }

        public void RestartAsAdmin()
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            string applicationPath = $@"{Directory.GetCurrentDirectory()}\{processName}.exe";

            ProcessStartInfo elevated = new ProcessStartInfo(applicationPath)
            {
                UseShellExecute = true,
                Verb = "runas"
            };

            Process.Start(elevated);
            Environment.Exit(0);
        }
    }
}
