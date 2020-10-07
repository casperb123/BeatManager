using BeatManager.Properties;
using BeatManager.UserControls;
using BeatManager.UserControls.Navigation;
using BeatSaverApi.Entities;
using GitHubUpdater;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Settings = BeatManager.Entities.Settings;
using Version = GitHubUpdater.Version;

namespace BeatManager.ViewModels
{
    public class MainWindowViewModel
    {
        private bool localBeatmapsLoaded = false;
        private ProgressDialogController progressDialog;

        public readonly MainWindow MainWindow;
        public readonly MainWindowViewModel ViewModel;
        public readonly BeatmapLocalUserControl LocalUserControl;
        public readonly BeatmapOnlineUserControl OnlineUserControl;
        public readonly SettingsUserControl SettingsUserControl;
        public readonly BeatmapLocalDetailsUserControl LocalDetailsUserControl;
        public readonly BeatmapOnlineDetailsUserControl OnlineDetailsUserControl;
        public readonly NavigationBeatmapsUserControl NavigationBeatmapsUserControl;

        public bool ShowLocalDetails;
        public bool ShowOnlineDetails;
        public bool UpdateAvailable;
        public bool UpdateDownloaded;
        public readonly Updater Updater;
        public bool IsLoaded;
        public bool OnlineSongChanged;
        public bool LocalSongChanged;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            LocalUserControl = new BeatmapLocalUserControl(mainWindow);
            OnlineUserControl = new BeatmapOnlineUserControl(mainWindow);
            SettingsUserControl = new SettingsUserControl(mainWindow);
            LocalDetailsUserControl = new BeatmapLocalDetailsUserControl(mainWindow);
            OnlineDetailsUserControl = new BeatmapOnlineDetailsUserControl(mainWindow);
            NavigationBeatmapsUserControl = new NavigationBeatmapsUserControl();

            NavigationBeatmapsUserControl.ViewModel.LocalEvent += NavigationBeatmapsUserControl_LocalEvent;
            NavigationBeatmapsUserControl.ViewModel.OnlineEvent += NavigationBeatmapsUserControl_OnlineEvent;

            try
            {
                Updater = new Updater("casperb123", "BeatManager", Resources.GitHubToken);
                Updater.UpdateAvailable += Updater_UpdateAvailable;
                Updater.DownloadingStarted += Updater_DownloadingStarted;
                Updater.DownloadingProgressed += Updater_DownloadingProgressed;
                Updater.DownloadingCompleted += Updater_DownloadingCompleted;
                Updater.DownloadingFailed += Updater_DownloadingFailed;
                Updater.InstallationFailed += Updater_InstallationFailed;

                CheckForUpdates();
            }
            catch (Exception e)
            {
                if (e.InnerException is null)
                    mainWindow.ShowMessageAsync("Checking for updates failed", $"There was an error while checking for updates.\n\n" +
                                                                               $"Error:\n" +
                                                                               $"{e.Message}").ConfigureAwait(false);
                else
                    mainWindow.ShowMessageAsync("Checking for updates failed", $"There was an error while checking for updates.\n\n" +
                                                                               $"Error:\n" +
                                                                               $"{e.InnerException.Message}").ConfigureAwait(false);
            }
        }

        private void NavigationBeatmapsUserControl_LocalEvent(object sender, EventArgs e)
        {
            ShowLocalPage();
        }

        private void NavigationBeatmapsUserControl_OnlineEvent(object sender, EventArgs e)
        {
            ShowOnlinePage();
        }

        private async void CheckForUpdates()
        {
            if (Settings.CurrentSettings.CheckForUpdates)
            {
                Version version = await Updater.CheckForUpdatesAsync();
                if (version.IsCurrentVersion)
                    Updater.DeleteUpdateFiles();
            }
            else if (Updater.IsUpdateDownloaded())
            {
                UpdateDownloaded = true;
                MainWindow.buttonUpdate.Content = "Update downloaded";
            }
            else
                Updater.DeleteUpdateFiles();
        }

        private async void Updater_InstallationFailed(object sender, ExceptionEventArgs<Exception> e)
        {
            await MainWindow.ShowMessageAsync("Installation failed", "Installing the update failed.\n\n" +
                                                                     "Error:\n" +
                                                                     $"{e.Message}");
        }

        private async void Updater_DownloadingFailed(object sender, ExceptionEventArgs<Exception> e)
        {
            await MainWindow.ShowMessageAsync("Downloading failed", "Downloading the update failed.\n\n" +
                                                                    "Error:\n" +
                                                                    $"{e.Message}");
        }

        private async void Updater_DownloadingCompleted(object sender, VersionEventArgs e)
        {
            Settings.CurrentSettings.NotifyUpdates = true;
            Settings.CurrentSettings.Save();
            await progressDialog.CloseAsync();
            await NotifyDownloaded(e.CurrentVersion, e.LatestVersion, e.Changelog);
        }

        private void Updater_DownloadingProgressed(object sender, DownloadProgressEventArgs e)
        {
            progressDialog.SetProgress(e.ProgressPercent);
            progressDialog.SetMessage($"Estimated time left: {e.TimeLeft} ({e.Downloaded} of {e.ToDownload} downloaded)\n" +
                                      $"Time spent: {e.TimeSpent}");
        }

        private async void Updater_DownloadingStarted(object sender, DownloadStartedEventArgs e)
        {
            progressDialog = await MainWindow.ShowProgressAsync($"Downloading update - {e.Version}", "Estimated time left: 0 sec (0 kb of 0 kb downloaded)\n" +
                                                                                                     "Time spent: 0 sec");
            progressDialog.Minimum = 0;
            progressDialog.Maximum = 100;
        }

        private async void Updater_UpdateAvailable(object sender, VersionEventArgs e)
        {
            if (e.UpdateDownloaded)
            {
                if (Settings.CurrentSettings.NotifyUpdates || UpdateDownloaded)
                    await NotifyDownloaded(e.CurrentVersion, e.LatestVersion, e.Changelog);
                else
                {
                    UpdateDownloaded = true;
                    MainWindow.buttonUpdate.Content = "Update downloaded";
                }
            }
            else
            {
                if (Settings.CurrentSettings.NotifyUpdates || UpdateAvailable)
                {
                    string message = $"An update is available, would you like to download it now?\n" +
                                     $"Current version: {e.CurrentVersion}\n" +
                                     $"Latest version: {e.LatestVersion}\n\n" +
                                     $"Changelog:\n" +
                                     $"{e.Changelog}";

                    MessageDialogResult result = await MainWindow.ShowMessageAsync("Update available", message, MessageDialogStyle.AffirmativeAndNegative);

                    if (result == MessageDialogResult.Affirmative)
                    {
                        Settings.CurrentSettings.NotifyUpdates = true;
                        Settings.CurrentSettings.Save();

                        Updater.DownloadUpdate();
                    }
                    else
                    {
                        Settings.CurrentSettings.NotifyUpdates = false;
                        Settings.CurrentSettings.Save();
                        UpdateAvailable = true;
                        MainWindow.buttonUpdate.Content = "Update available";
                    }
                }
                else
                {
                    if (Settings.CurrentSettings.NotifyUpdates)
                    {
                        Settings.CurrentSettings.NotifyUpdates = false;
                        Settings.CurrentSettings.Save();
                    }

                    UpdateAvailable = true;
                    UpdateDownloaded = false;
                    MainWindow.buttonUpdate.Content = "Update available";
                }
            }
        }

        public async Task NotifyDownloaded(Version currentVersion, Version latestVersion, string changelog = null)
        {
            string message = $"An update has been downloaded. Would you like to install it now?\n\n" +
                             $"Current version: {currentVersion}\n" +
                             $"Downloaded version: {latestVersion}";

            if (changelog != null)
                message += $"\n\nChangelog:\n" +
                           $"{changelog}";

            MessageDialogResult result = await MainWindow.ShowMessageAsync("Update downloaded", message, MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
            {
                Settings.CurrentSettings.NotifyUpdates = true;
                Settings.CurrentSettings.Save();

                Updater.InstallUpdate();
            }
            else
            {
                Settings.CurrentSettings.NotifyUpdates = false;
                Settings.CurrentSettings.Save();
                UpdateDownloaded = true;
                UpdateAvailable = false;

                MainWindow.buttonUpdate.Content = "Update downloaded";
            }
        }

        public void ShowLocalPage()
        {
            if (MainWindow.userControlMain.Content == LocalDetailsUserControl)
                ShowLocalDetails = false;

            if (ShowLocalDetails && !SettingsUserControl.ViewModel.SongsPathChanged)
                MainWindow.userControlMain.Content = LocalDetailsUserControl;
            else
            {
                MainWindow.userControlMain.Content = LocalUserControl;

                if (!localBeatmapsLoaded ||
                    OnlineSongChanged ||
                    SettingsUserControl.ViewModel.SongsPathChanged)
                {
                    localBeatmapsLoaded = true;
                    OnlineSongChanged = false;

                    if (LocalUserControl.ViewModel.LocalBeatmaps is null || SettingsUserControl.ViewModel.SongsPathChanged)
                        LocalUserControl.ViewModel.GetBeatmaps();
                    else
                        LocalUserControl.ViewModel.GetBeatmaps(LocalUserControl.ViewModel.LocalBeatmaps);

                    SettingsUserControl.ViewModel.SongsPathChanged = false;
                }
            }
        }

        public void ShowOnlinePage()
        {
            if (MainWindow.userControlMain.Content == OnlineDetailsUserControl)
                ShowOnlineDetails = false;

            if (ShowOnlineDetails && !SettingsUserControl.ViewModel.SongsPathChanged)
                MainWindow.userControlMain.Content = OnlineDetailsUserControl;
            else
            {
                if (!OnlineUserControl.ViewModel.IsLoaded)
                {
                    OnlineUserControl.radioButtonHot.IsChecked = true;
                    OnlineUserControl.ViewModel.IsLoaded = true;
                }
                else if (LocalSongChanged)
                {
                    if (OnlineUserControl.ViewModel.OnlineBeatmaps != null)
                    {
                        MapSort mapSort = OnlineUserControl.ViewModel.CurrentMapSort;
                        if (mapSort == MapSort.Search)
                            OnlineUserControl.ViewModel.GetBeatmaps(OnlineUserControl.textBoxSearch.Text, OnlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                        else
                            OnlineUserControl.ViewModel.GetBeatmaps(mapSort, OnlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                    }

                    LocalSongChanged = false;
                }

                MainWindow.userControlMain.Content = OnlineUserControl;
            }
        }

        public void ShowSettingsPage()
        {
            MainWindow.userControlNavigation.Content = null;
            MainWindow.userControlMain.Content = SettingsUserControl;
        }

        public async Task DownloadSong(string key)
        {
            await MainWindow.Dispatcher.Invoke(async () =>
            {
                OnlineBeatmap onlineBeatmap = await App.BeatSaverApi.GetBeatmap(key);

                if (onlineBeatmap != null)
                {
                    try
                    {
                        MainWindow.ToggleLoading(true, "Downloading Song", onlineBeatmap.Metadata.FullSongName);
                        await App.BeatSaverApi.DownloadSong(onlineBeatmap);
                        OnlineSongChanged = true;
                        if (OnlineUserControl.ViewModel.OnlineBeatmaps != null && OnlineUserControl.ViewModel.OnlineBeatmaps.Maps.Any(x => x.Key == key))
                            LocalSongChanged = true;

                        MainWindow.ToggleLoading(false);
                        if (MainWindow.userControlMain.Content == LocalUserControl || MainWindow.userControlMain.Content == LocalDetailsUserControl)
                            ShowLocalPage();
                        else if (MainWindow.userControlMain.Content == OnlineUserControl || MainWindow.userControlMain.Content == OnlineDetailsUserControl)
                            ShowOnlinePage();
                    }
                    catch (Exception)
                    {
                        MainWindow.ToggleLoading(false);
                        await MainWindow.ShowMessageAsync("Downloading failed", "Downloading the song failed, please try again.");
                    }
                }
            });
        }
    }
}
