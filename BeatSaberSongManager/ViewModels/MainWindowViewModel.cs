using BeatSaberSongManager.Entities;
using BeatSaberSongManager.Properties;
using BeatSaberSongManager.UserControls;
using BeatSaverApi.Entities;
using GitHubUpdater;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using Version = GitHubUpdater.Version;

namespace BeatSaberSongManager.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly MainWindow mainWindow;
        private bool localBeatmapsLoaded = false;
        private ProgressDialogController progressDialog;

        public readonly MainWindowViewModel ViewModel;
        public readonly BeatmapLocalUserControl LocalUserControl;
        public readonly BeatmapOnlineUserControl OnlineUserControl;
        public readonly SettingsUserControl SettingsUserControl;
        public readonly BeatmapLocalDetailsUserControl LocalDetailsUserControl;
        public readonly BeatmapOnlineDetailsUserControl OnlineDetailsUserControl;

        public bool ShowLocalDetails;
        public bool ShowOnlineDetails;
        public bool UpdateAvailable;
        public bool UpdateDownloaded;
        public readonly Updater Updater;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            LocalUserControl = new BeatmapLocalUserControl(mainWindow);
            OnlineUserControl = new BeatmapOnlineUserControl(mainWindow);
            SettingsUserControl = new SettingsUserControl();
            LocalDetailsUserControl = new BeatmapLocalDetailsUserControl(mainWindow);
            OnlineDetailsUserControl = new BeatmapOnlineDetailsUserControl(mainWindow);

            try
            {
                Updater = new Updater("casperb123", "BeatSaberSongManager", Resources.GitHubToken);
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
                mainWindow.buttonUpdate.Content = "Update downloaded";
            }
            else
                Updater.DeleteUpdateFiles();
        }

        private async void Updater_InstallationFailed(object sender, ExceptionEventArgs<Exception> e)
        {
            await mainWindow.ShowMessageAsync("Installation failed", "Installing the update failed.\n\n" +
                                                                     "Error:\n" +
                                                                     $"{e.Message}");
        }

        private async void Updater_DownloadingFailed(object sender, ExceptionEventArgs<Exception> e)
        {
            await mainWindow.ShowMessageAsync("Downloading failed", "Downloading the update failed.\n\n" +
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
            progressDialog = await mainWindow.ShowProgressAsync($"Downloading update - {e.Version}", "Estimated time left: 0 sec (0 kb of 0 kb downloaded)\n" +
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
                    mainWindow.buttonUpdate.Content = "Update downloaded";
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

                    MessageDialogResult result = await mainWindow.ShowMessageAsync("Update available", message, MessageDialogStyle.AffirmativeAndNegative);

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
                        mainWindow.buttonUpdate.Content = "Update available";
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
                    mainWindow.buttonUpdate.Content = "Update available";
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

            MessageDialogResult result = await mainWindow.ShowMessageAsync("Update downloaded", message, MessageDialogStyle.AffirmativeAndNegative);

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

                mainWindow.buttonUpdate.Content = "Update downloaded";
            }
        }

        public void ShowLocalPage()
        {
            if (mainWindow.transitionControl.Content == LocalDetailsUserControl)
                ShowLocalDetails = false;

            if (ShowLocalDetails && !SettingsUserControl.ViewModel.SongsPathChanged)
                mainWindow.transitionControl.Content = LocalDetailsUserControl;
            else
            {
                mainWindow.transitionControl.Content = LocalUserControl;

                if (!localBeatmapsLoaded ||
                    OnlineUserControl.ViewModel.SongChanged ||
                    SettingsUserControl.ViewModel.SongsPathChanged)
                {
                    localBeatmapsLoaded = true;
                    OnlineUserControl.ViewModel.SongChanged = false;

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
            if (mainWindow.transitionControl.Content == OnlineDetailsUserControl)
                ShowOnlineDetails = false;

            if (ShowOnlineDetails && !SettingsUserControl.ViewModel.SongsPathChanged)
                mainWindow.transitionControl.Content = OnlineDetailsUserControl;
            else
            {
                if (!OnlineUserControl.ViewModel.IsLoaded)
                {
                    OnlineUserControl.radioButtonHot.IsChecked = true;
                    OnlineUserControl.ViewModel.IsLoaded = true;
                }
                else if (LocalUserControl.ViewModel.SongDeleted)
                {
                    if (OnlineUserControl.ViewModel.OnlineBeatmaps != null)
                    {
                        MapSort mapSort = OnlineUserControl.ViewModel.CurrentMapSort;
                        if (mapSort == MapSort.Search)
                            OnlineUserControl.ViewModel.GetBeatmaps(OnlineUserControl.textBoxSearch.Text, OnlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                        else
                            OnlineUserControl.ViewModel.GetBeatmaps(mapSort, OnlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                    }

                    LocalUserControl.ViewModel.SongDeleted = false;
                }

                mainWindow.transitionControl.Content = OnlineUserControl;
            }
        }

        public void ShowSettingsPage()
        {
            mainWindow.transitionControl.Content = SettingsUserControl;
        }
    }
}
