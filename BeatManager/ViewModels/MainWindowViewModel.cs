using BeatManager.Properties;
using BeatManager.UserControls;
using BeatManager.UserControls.BeatSaver;
using BeatManager.UserControls.ModelSaber;
using BeatManager.UserControls.Navigation;
using BeatSaver.Entities;
using GitHubUpdater;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Settings = BeatManager.Entities.Settings;
using Version = GitHubUpdater.Version;

namespace BeatManager.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ProgressDialogController progressDialog;
        private int downloads;

        public readonly MainWindow MainWindow;
        public readonly MainWindowViewModel ViewModel;
        public readonly BeatmapLocalUserControl BeatmapLocalUserControl;
        public readonly BeatmapOnlineUserControl BeatmapOnlineUserControl;
        public readonly SettingsUserControl SettingsUserControl;
        public readonly BeatmapLocalDetailsUserControl BeatmapLocalDetailsUserControl;
        public readonly BeatmapOnlineDetailsUserControl BeatmapOnlineDetailsUserControl;
        public readonly ModelSaberBaseOnlineUserControl SaberOnlineUserControl;
        public readonly ModelSaberBaseLocalUserControl SaberLocalUserControl;
        public readonly NavigationBeatmapsUserControl NavigationBeatmapsUserControl;
        public readonly NavigationSabersUserControl NavigationSabersUserControl;
        public readonly DownloadsUserControl DownloadsUserControl;

        public bool ShowLocalBeatmapDetails;
        public bool ShowOnlineBeatmapDetails;
        public bool ShowLocalSaberDetails;
        public bool ShowOnlineSaberDetails;
        public bool UpdateAvailable;
        public bool UpdateDownloaded;
        public readonly Updater Updater;
        public bool IsLoaded;
        public bool OnlineBeatmapChanged;
        public bool LocalBeatmapChanged;
        public bool OnlineSaberChanged;
        public bool LocalSaberChanged;
        public bool OnlineAvatarChanged;
        public bool LocalAvatarChanged;
        public bool OnlinePlatformChanged;
        public bool LocalPlatformChanged;
        public bool OnlineBloqChanged;
        public bool LocalBloqChanged;

        public int Downloads
        {
            get { return downloads; }
            set
            {
                downloads = value;
                OnPropertyChanged(nameof(Downloads));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            BeatmapLocalUserControl = new BeatmapLocalUserControl(mainWindow);
            BeatmapOnlineUserControl = new BeatmapOnlineUserControl(mainWindow);
            SettingsUserControl = new SettingsUserControl(mainWindow);
            BeatmapLocalDetailsUserControl = new BeatmapLocalDetailsUserControl(mainWindow);
            BeatmapOnlineDetailsUserControl = new BeatmapOnlineDetailsUserControl(mainWindow);
            SaberOnlineUserControl = new ModelSaberBaseOnlineUserControl(mainWindow);
            SaberLocalUserControl = new ModelSaberBaseLocalUserControl(mainWindow);
            NavigationBeatmapsUserControl = new NavigationBeatmapsUserControl();
            NavigationSabersUserControl = new NavigationSabersUserControl();
            
            DownloadsUserControl = new DownloadsUserControl();

            NavigationBeatmapsUserControl.ViewModel.LocalEvent += NavigationBeatmaps_LocalEvent;
            NavigationBeatmapsUserControl.ViewModel.OnlineEvent += NavigationBeatmaps_OnlineEvent;
            NavigationSabersUserControl.ViewModel.LocalEvent += NavigationSabers_LocalEvent;
            NavigationSabersUserControl.ViewModel.OnlineEvent += NavigationSabers_OnlineEvent;

            App.BeatSaverApi.DownloadStarted += BeatSaverApi_DownloadStarted;
            App.BeatSaverApi.DownloadFailed += BeatSaverApi_DownloadFailed;

            App.ModelSaberApi.DownloadStarted += ModelSaberApi_DownloadStarted;
            App.ModelSaberApi.DownloadFailed += ModelSaberApi_DownloadFailed;

            Updater = new Updater("casperb123", "BeatManager", Resources.GitHubToken);
            Updater.UpdateAvailable += Updater_UpdateAvailable;
            Updater.DownloadingStarted += Updater_DownloadingStarted;
            Updater.DownloadingProgressed += Updater_DownloadingProgressed;
            Updater.DownloadingCompleted += Updater_DownloadingCompleted;
            Updater.DownloadingFailed += Updater_DownloadingFailed;
            Updater.InstallationFailed += Updater_InstallationFailed;

            CheckForUpdates();
        }

        private void BeatSaverApi_DownloadFailed(object sender, BeatSaver.Events.DownloadFailedEventArgs e)
        {
            if (MainWindow.userControlMain.Content != DownloadsUserControl)
                Downloads++;
        }

        private void BeatSaverApi_DownloadStarted(object sender, BeatSaver.Events.DownloadStartedEventArgs e)
        {
            if (MainWindow.userControlMain.Content != DownloadsUserControl)
                Downloads++;
        }

        private void ModelSaberApi_DownloadFailed(object sender, global::ModelSaber.Events.DownloadFailedEventArgs e)
        {
            if (MainWindow.userControlMain.Content != DownloadsUserControl)
                Downloads++;
        }

        private void ModelSaberApi_DownloadStarted(object sender, global::ModelSaber.Events.DownloadStartedEventArgs e)
        {
            if (MainWindow.userControlMain.Content != DownloadsUserControl)
                Downloads++;
        }

        private void NavigationBeatmaps_LocalEvent(object sender, EventArgs e)
        {
            ShowLocalBeatmapsPage();
        }

        private void NavigationBeatmaps_OnlineEvent(object sender, EventArgs e)
        {
            ShowOnlineBeatmapsPage();
        }

        private void NavigationSabers_OnlineEvent(object sender, EventArgs e)
        {
            ShowOnlineSabersPage();
        }

        private void NavigationSabers_LocalEvent(object sender, EventArgs e)
        {
            ShowLocalSabersPage();
        }

        private async void CheckForUpdates()
        {
            try
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
            catch (HttpRequestException) { }
            catch (Exception e)
            {
                if (string.Equals(e.Message, e.InnerException.Message))
                {
                    await MainWindow.ShowMessageAsync("Checking for updates failed", $"There was an error while checking for updates.\n\n" +
                                                                                     $"Error:\n" +
                                                                                     $"{e.Message}");
                }
                else
                {
                    await MainWindow.ShowMessageAsync("Checking for updates failed", $"There was an error while checking for updates.\n\n" +
                                                                                     $"Error:\n" +
                                                                                     $"{e.Message} ({e.InnerException.Message})");
                }
            }
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
            progressDialog = await MainWindow.ShowProgressAsync($"Downloading update - {e.Version}", "Estimated time left: 0 sec (0 kB of 0 kB downloaded)\n" +
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

        public void ShowLocalBeatmapsPage()
        {
            if (MainWindow.userControlMain.Content == BeatmapLocalDetailsUserControl)
                ShowLocalBeatmapDetails = false;

            NavigationBeatmapsUserControl.radioButtonLocal.IsChecked = true;
            if (ShowLocalBeatmapDetails && !SettingsUserControl.ViewModel.SongsPathChanged)
                MainWindow.userControlMain.Content = BeatmapLocalDetailsUserControl;
            else
            {
                MainWindow.userControlMain.Content = BeatmapLocalUserControl;

                if (!BeatmapLocalUserControl.ViewModel.IsLoaded ||
                    OnlineBeatmapChanged ||
                    SettingsUserControl.ViewModel.SongsPathChanged)
                {
                    BeatmapLocalUserControl.ViewModel.IsLoaded = true;
                    OnlineBeatmapChanged = false;

                    if (BeatmapLocalUserControl.ViewModel.LocalBeatmaps is null || SettingsUserControl.ViewModel.SongsPathChanged)
                        BeatmapLocalUserControl.ViewModel.GetBeatmaps();
                    else
                        BeatmapLocalUserControl.ViewModel.GetBeatmaps(BeatmapLocalUserControl.ViewModel.LocalBeatmaps);

                    SettingsUserControl.ViewModel.SongsPathChanged = false;
                }
            }
        }

        public void ShowOnlineBeatmapsPage()
        {
            if (MainWindow.userControlMain.Content == BeatmapOnlineDetailsUserControl)
                ShowOnlineBeatmapDetails = false;

            NavigationBeatmapsUserControl.radioButtonOnline.IsChecked = true;
            if (ShowOnlineBeatmapDetails && !SettingsUserControl.ViewModel.SongsPathChanged)
                MainWindow.userControlMain.Content = BeatmapOnlineDetailsUserControl;
            else
            {
                if (!BeatmapOnlineUserControl.ViewModel.IsLoaded)
                {
                    BeatmapOnlineUserControl.radioButtonHot.IsChecked = true;
                    BeatmapOnlineUserControl.ViewModel.IsLoaded = true;
                }
                else if (LocalBeatmapChanged)
                {
                    if (BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps != null)
                    {
                        MapSort mapSort = BeatmapOnlineUserControl.ViewModel.CurrentMapSort;
                        if (mapSort == MapSort.Search)
                            BeatmapOnlineUserControl.ViewModel.GetBeatmaps(BeatmapOnlineUserControl.textBoxSearch.Text, BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                        else
                            BeatmapOnlineUserControl.ViewModel.GetBeatmaps(mapSort, BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                    }

                    LocalBeatmapChanged = false;
                }

                MainWindow.userControlMain.Content = BeatmapOnlineUserControl;
            }
        }

        public void ShowOnlineSabersPage()
        {
            //if (MainWindow.userControlMain.Content == BeatmapOnlineDetailsUserControl)
            //    ShowOnlineBeatmapDetails = false;

            NavigationSabersUserControl.radioButtonOnline.IsChecked = true;
            if (ShowOnlineSaberDetails && !SettingsUserControl.ViewModel.SongsPathChanged)
            {
                // Show online saber details
            }
            else
            {
                if (!SaberOnlineUserControl.ViewModel.IsLoaded)
                {
                    SaberOnlineUserControl.ViewModel.GetSabers();
                    SaberOnlineUserControl.ViewModel.IsLoaded = true;
                }
                else if (LocalSaberChanged)
                {
                    SaberOnlineUserControl.ViewModel.GetSabers();
                    LocalSaberChanged = false;
                }

                MainWindow.userControlMain.Content = SaberOnlineUserControl;
            }
        }

        public void ShowLocalSabersPage()
        {
            //if (MainWindow.userControlMain.Content == BeatmapLocalDetailsUserControl)
            //    ShowLocalBeatmapDetails = false;

            NavigationSabersUserControl.radioButtonLocal.IsChecked = true;
            if (ShowLocalSaberDetails && !SettingsUserControl.ViewModel.SongsPathChanged)
            {
                //MainWindow.userControlMain.Content = BeatmapLocalDetailsUserControl;
            }
            else
            {
                MainWindow.userControlMain.Content = SaberLocalUserControl;
                //BeatmapLocalDetailsUserControl.ViewModel.CloseBigCover();

                if (!SaberLocalUserControl.ViewModel.IsLoaded ||
                    OnlineSaberChanged ||
                    SettingsUserControl.ViewModel.SongsPathChanged)
                {
                    SaberLocalUserControl.ViewModel.IsLoaded = true;
                    OnlineSaberChanged = false;

                    if (SettingsUserControl.ViewModel.SongsPathChanged)
                        SaberLocalUserControl.ViewModel.GetModels(false);
                    else
                        SaberLocalUserControl.ViewModel.GetModels(true);

                    SettingsUserControl.ViewModel.SongsPathChanged = false;
                }
            }
        }

        public void ShowDownloadsPage()
        {
            MainWindow.userControlNavigation.Content = null;
            MainWindow.userControlMain.Content = DownloadsUserControl;
            Downloads = 0;
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
                try
                {
                    OnlineBeatmap onlineBeatmap = await App.BeatSaverApi.GetBeatmap(key);
                    bool isValid = await App.BeatSaverApi.DownloadSong(onlineBeatmap);

                    if (isValid)
                    {
                        OnlineBeatmapChanged = true;
                        if (BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps != null && BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps.Maps.Any(x => x.Key == key))
                            LocalBeatmapChanged = true;
                        if (MainWindow.userControlMain.Content == BeatmapLocalUserControl || MainWindow.userControlMain.Content == BeatmapLocalDetailsUserControl)
                            ShowLocalBeatmapsPage();
                        else if (MainWindow.userControlMain.Content == BeatmapOnlineUserControl || MainWindow.userControlMain.Content == BeatmapOnlineDetailsUserControl)
                            ShowOnlineBeatmapsPage();
                    }
                }
                catch (InvalidOperationException e)
                {
                    MainWindow.ToggleLoading(false);
                    string errorMessage = e.Message;
                    if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                        errorMessage += $" ({e.InnerException.Message})";

                    await MainWindow.ShowMessageAsync("Downloading failed", "Downloading the song failed with the following error\n\n" +
                                                                            "Error:\n" +
                                                                            $"{errorMessage}");
                }
                catch (Exception e)
                {
                    MainWindow.ToggleLoading(false);
                    string errorMessage = e.Message;
                    if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                        errorMessage += $" ({e.InnerException.Message})";

                    MessageDialogResult result = await MainWindow.ShowMessageAsync("Downloading failed", "Downloading the song failed, would you like to try again?\n\n" +
                                                                                                         "Error:\n" +
                                                                                                         $"{errorMessage}", MessageDialogStyle.AffirmativeAndNegative);

                    if (result == MessageDialogResult.Affirmative)
                        await DownloadSong(key);
                }
            });
        }

        public void OpenBigCover(ImageSource image)
        {
            MainWindow.imageCoverImage.Source = image;
            ((Storyboard)MainWindow.Resources["OpenCover"]).Begin();
            MainWindow.gridCoverImage.Visibility = Visibility.Visible;
        }

        public void CloseBigCover()
        {
            ((Storyboard)MainWindow.Resources["CloseCover"]).Begin();
            MainWindow.gridCoverImage.Visibility = Visibility.Hidden;
            MainWindow.gridCoverImage.Opacity = 0;
        }
    }
}
