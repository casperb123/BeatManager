using BeatManager.Properties;
using BeatManager.UserControls;
using BeatManager.UserControls.BeatSaver;
using BeatManager.UserControls.ModelSaber;
using BeatManager.UserControls.Navigation;
using BeatSaver.Entities;
using GitHubUpdater;
using MahApps.Metro.Controls.Dialogs;
using ModelSaber.Entities;
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

        public MainWindow MainWindow { get; private set; }
        public MainWindowViewModel ViewModel { get; private set; }
        public BeatmapLocalUserControl BeatmapLocalUserControl { get; private set; }
        public BeatmapOnlineUserControl BeatmapOnlineUserControl { get; private set; }
        public BeatmapLocalDetailsUserControl BeatmapLocalDetailsUserControl { get; private set; }
        public BeatmapOnlineDetailsUserControl BeatmapOnlineDetailsUserControl { get; private set; }
        public ModelSaberOnlineUserControl SaberOnlineUserControl { get; private set; }
        public ModelSaberLocalUserControl SaberLocalUserControl { get; private set; }
        public ModelSaberOnlineUserControl AvatarOnlineUserControl { get; private set; }
        public ModelSaberLocalUserControl AvatarLocalUserControl { get; private set; }
        public ModelSaberOnlineUserControl PlatformOnlineUserControl { get; private set; }
        public ModelSaberLocalUserControl PlatformLocalUserControl { get; private set; }
        public ModelSaberOnlineUserControl BloqOnlineUserControl { get; private set; }
        public ModelSaberLocalUserControl BloqLocalUserControl { get; private set; }
        public ModelSaberOnlineDetailsUserControl ModelSaberOnlineDetailsUserControl { get; private set; }
        public ModelSaberLocalDetailsUserControl ModelSaberLocalDetailsUserControl { get; private set; }
        public NavigationBeatmapsUserControl NavigationBeatmapsUserControl { get; private set; }
        public NavigationModelSaberUserControl NavigationSabersUserControl { get; private set; }
        public NavigationModelSaberUserControl NavigationAvatarsUserControl { get; private set; }
        public NavigationModelSaberUserControl NavigationPlatformsUserControl { get; private set; }
        public NavigationModelSaberUserControl NavigationBloqsUserControl { get; private set; }
        public SettingsUserControl SettingsUserControl { get; private set; }
        public DownloadsUserControl DownloadsUserControl { get; private set; }

        public bool UpdateAvailable;
        public bool UpdateDownloaded;
        public readonly Updater Updater;
        public bool IsLoaded;

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
            SaberOnlineUserControl = new ModelSaberOnlineUserControl(mainWindow, ModelType.Saber);
            SaberLocalUserControl = new ModelSaberLocalUserControl(mainWindow, ModelType.Saber);
            AvatarOnlineUserControl = new ModelSaberOnlineUserControl(mainWindow, ModelType.Avatar);
            AvatarLocalUserControl = new ModelSaberLocalUserControl(mainWindow, ModelType.Avatar);
            PlatformOnlineUserControl = new ModelSaberOnlineUserControl(mainWindow, ModelType.Platform);
            PlatformLocalUserControl = new ModelSaberLocalUserControl(mainWindow, ModelType.Platform);
            BloqOnlineUserControl = new ModelSaberOnlineUserControl(mainWindow, ModelType.Bloq);
            BloqLocalUserControl = new ModelSaberLocalUserControl(mainWindow, ModelType.Bloq);
            ModelSaberOnlineDetailsUserControl = new ModelSaberOnlineDetailsUserControl(mainWindow);
            ModelSaberLocalDetailsUserControl = new ModelSaberLocalDetailsUserControl(mainWindow);
            NavigationBeatmapsUserControl = new NavigationBeatmapsUserControl();
            NavigationSabersUserControl = new NavigationModelSaberUserControl(ModelType.Saber);
            NavigationAvatarsUserControl = new NavigationModelSaberUserControl(ModelType.Avatar);
            NavigationPlatformsUserControl = new NavigationModelSaberUserControl(ModelType.Platform);
            NavigationBloqsUserControl = new NavigationModelSaberUserControl(ModelType.Bloq);

            DownloadsUserControl = new DownloadsUserControl();

            NavigationBeatmapsUserControl.ViewModel.LocalEvent += NavigationBeatmaps_LocalEvent;
            NavigationBeatmapsUserControl.ViewModel.OnlineEvent += NavigationBeatmaps_OnlineEvent;
            NavigationSabersUserControl.ViewModel.LocalEvent += NavigationSabers_LocalEvent;
            NavigationSabersUserControl.ViewModel.OnlineEvent += NavigationSabers_OnlineEvent;
            NavigationAvatarsUserControl.ViewModel.LocalEvent += NavigationAvatars_LocalEvent;
            NavigationAvatarsUserControl.ViewModel.OnlineEvent += NavigationAvatars_OnlineEvent;
            NavigationPlatformsUserControl.ViewModel.LocalEvent += NavigationPlatforms_LocalEvent;
            NavigationPlatformsUserControl.ViewModel.OnlineEvent += NavigationPlatforms_OnlineEvent;
            NavigationBloqsUserControl.ViewModel.LocalEvent += NavigationBloqs_LocalEvent;
            NavigationBloqsUserControl.ViewModel.OnlineEvent += NavigationBloqs_OnlineEvent;

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
            ShowOnlineModelSaberPage(ModelType.Saber);
        }

        private void NavigationSabers_LocalEvent(object sender, EventArgs e)
        {
            ShowLocalModelSaberPage(ModelType.Saber);
        }

        private void NavigationAvatars_OnlineEvent(object sender, EventArgs e)
        {
            ShowOnlineModelSaberPage(ModelType.Avatar);
        }

        private void NavigationAvatars_LocalEvent(object sender, EventArgs e)
        {
            ShowLocalModelSaberPage(ModelType.Avatar);
        }

        private void NavigationPlatforms_OnlineEvent(object sender, EventArgs e)
        {
            ShowOnlineModelSaberPage(ModelType.Platform);
        }

        private void NavigationPlatforms_LocalEvent(object sender, EventArgs e)
        {
            ShowLocalModelSaberPage(ModelType.Platform);
        }

        private void NavigationBloqs_OnlineEvent(object sender, EventArgs e)
        {
            ShowOnlineModelSaberPage(ModelType.Bloq);
        }

        private void NavigationBloqs_LocalEvent(object sender, EventArgs e)
        {
            ShowLocalModelSaberPage(ModelType.Bloq);
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
            NavigationBeatmapsUserControl.radioButtonLocal.IsChecked = true;
            MainWindow.userControlMain.Content = BeatmapLocalUserControl;

            if (!BeatmapLocalUserControl.ViewModel.IsLoaded ||
                BeatmapOnlineUserControl.ViewModel.BeatmapChanged ||
                SettingsUserControl.ViewModel.SongsPathChanged)
            {
                BeatmapLocalUserControl.ViewModel.IsLoaded = true;
                BeatmapOnlineUserControl.ViewModel.BeatmapChanged = false;

                if (BeatmapLocalUserControl.ViewModel.LocalBeatmaps is null || SettingsUserControl.ViewModel.SongsPathChanged)
                    BeatmapLocalUserControl.ViewModel.GetBeatmaps();
                else
                    BeatmapLocalUserControl.ViewModel.GetBeatmaps(BeatmapLocalUserControl.ViewModel.LocalBeatmaps);

                SettingsUserControl.ViewModel.SongsPathChanged = false;
            }
        }

        public void ShowOnlineBeatmapsPage()
        {
            NavigationBeatmapsUserControl.radioButtonOnline.IsChecked = true;

            if (!BeatmapOnlineUserControl.ViewModel.IsLoaded)
            {
                BeatmapOnlineUserControl.radioButtonHot.IsChecked = true;
                BeatmapOnlineUserControl.ViewModel.IsLoaded = true;
            }
            else if (BeatmapLocalUserControl.ViewModel.BeatmapChanged)
            {
                if (BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps != null)
                {
                    MapSort mapSort = BeatmapOnlineUserControl.ViewModel.CurrentMapSort;
                    if (mapSort == MapSort.Search)
                        BeatmapOnlineUserControl.ViewModel.GetBeatmaps(BeatmapOnlineUserControl.textBoxSearch.Text, BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                    else
                        BeatmapOnlineUserControl.ViewModel.GetBeatmaps(mapSort, BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                }

                BeatmapLocalUserControl.ViewModel.BeatmapChanged = false;
            }

            MainWindow.userControlMain.Content = BeatmapOnlineUserControl;
        }

        public void ShowOnlineModelSaberPage(ModelType modelType)
        {
            ModelSaberOnlineUserControl onlineUserControl = PropertyHelper.GetPropValue<ModelSaberOnlineUserControl>(this, $"{modelType}OnlineUserControl");
            ModelSaberLocalUserControl localUserControl = PropertyHelper.GetPropValue<ModelSaberLocalUserControl>(this, $"{modelType}LocalUserControl");
            NavigationModelSaberUserControl navigationUserControl = PropertyHelper.GetPropValue<NavigationModelSaberUserControl>(this, $"Navigation{modelType}sUserControl");

            navigationUserControl.radioButtonOnline.IsChecked = true;
            if (!onlineUserControl.ViewModel.IsLoaded)
            {
                onlineUserControl.ViewModel.GetModels();
                onlineUserControl.ViewModel.IsLoaded = true;
            }
            else if (localUserControl.ViewModel.ModelChanged)
            {
                onlineUserControl.ViewModel.GetModels();
                localUserControl.ViewModel.ModelChanged = false;
            }
            MainWindow.userControlMain.Content = onlineUserControl;
        }

        public void ShowLocalModelSaberPage(ModelType modelType)
        {
            ModelSaberOnlineUserControl onlineUserControl = PropertyHelper.GetPropValue<ModelSaberOnlineUserControl>(this, $"{modelType}OnlineUserControl");
            ModelSaberLocalUserControl localUserControl = PropertyHelper.GetPropValue<ModelSaberLocalUserControl>(this, $"{modelType}LocalUserControl");
            NavigationModelSaberUserControl navigationUserControl = PropertyHelper.GetPropValue<NavigationModelSaberUserControl>(this, $"Navigation{modelType}sUserControl");

            navigationUserControl.radioButtonLocal.IsChecked = true;
            MainWindow.userControlMain.Content = localUserControl;
            if (!localUserControl.ViewModel.IsLoaded ||
                onlineUserControl.ViewModel.ModelChanged ||
                SettingsUserControl.ViewModel.SongsPathChanged)
            {
                localUserControl.ViewModel.IsLoaded = true;
                onlineUserControl.ViewModel.ModelChanged = false;

                if (localUserControl.ViewModel.LocalModels is null || SettingsUserControl.ViewModel.SongsPathChanged)
                    localUserControl.ViewModel.GetModels();
                else
                    localUserControl.ViewModel.GetModels(localUserControl.ViewModel.LocalModels);

                SettingsUserControl.ViewModel.SongsPathChanged = false;
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
                        BeatmapOnlineUserControl.ViewModel.BeatmapChanged = true;
                        if (BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps != null && BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps.Maps.Any(x => x.Key == key))
                            BeatmapLocalUserControl.ViewModel.BeatmapChanged = true;
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
