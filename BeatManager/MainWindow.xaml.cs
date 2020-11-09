using BeatManager.Entities;
using BeatManager.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ModelSaber.Entities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Version = GitHubUpdater.Version;

namespace BeatManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow Instance;

        public readonly MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel(this);
            DataContext = ViewModel;
            Instance = this;

            NamedPipe<string> beatSaverPipe = new NamedPipe<string>(NamedPipe<string>.NameTypes.BeatSaver);
            beatSaverPipe.OnRequest += new NamedPipe<string>.Request(BeatSaverPipe_OnRequest);
            beatSaverPipe.Start();

            NamedPipe<string> modelSaberPipe = new NamedPipe<string>(NamedPipe<string>.NameTypes.ModelSaber);
            modelSaberPipe.OnRequest += new NamedPipe<string>.Request(ModelSaberPipe_OnRequest);
            modelSaberPipe.Start();
        }

        private async void BeatSaverPipe_OnRequest(string key)
        {
            await ViewModel.DownloadSong(key);
        }

        private async void ModelSaberPipe_OnRequest(string args)
        {
            int typeSlash = args.IndexOf("/");
            int idSlash = args.IndexOf("/", typeSlash + 1);
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            ModelType modelType = Enum.Parse<ModelType>(textInfo.ToTitleCase(args.Substring(0, typeSlash)));
            int id = int.Parse(args.Substring(typeSlash + 1, idSlash - (typeSlash + 1)));
            await ViewModel.DownloadModel(id, modelType);
        }

        public static void ToggleLoading(bool enabled, string title = null, string description = null)
        {
            if (enabled)
            {
                if (!string.IsNullOrWhiteSpace(title))
                {
                    Instance.textBlockLoadingTitle.Text = title;
                    Instance.stackPanelLoadingText.Visibility = Visibility.Visible;
                }
                if (!string.IsNullOrWhiteSpace(description))
                {
                    Instance.textBlockLoadingDescription.Text = description;
                    Instance.textBlockLoadingDescription.Visibility = Visibility.Visible;
                    Instance.stackPanelLoadingText.Visibility = Visibility.Visible;
                }

                Instance.progressRingLoading.IsActive = true;
                Instance.stackPanelLoading.Visibility = Visibility.Visible;
                Instance.rectangleLoading.Visibility = Visibility.Visible;
            }
            else
            {
                Instance.rectangleLoading.Visibility = Visibility.Hidden;
                Instance.stackPanelLoading.Visibility = Visibility.Hidden;
                Instance.stackPanelLoadingText.Visibility = Visibility.Hidden;
                Instance.textBlockLoadingDescription.Visibility = Visibility.Collapsed;
                Instance.progressRingLoading.IsActive = false;
            }
        }

        private void RadioButtonHome_Checked(object sender, RoutedEventArgs e)
        {
            userControlNavigation.Content = null;
            userControlMain.Content = null;
        }

        private void RadioButtonBeatmaps_Checked(object sender, RoutedEventArgs e)
        {
            userControlNavigation.Content = ViewModel.NavigationBeatmapsUserControl;

            if (ViewModel.NavigationBeatmapsUserControl.ViewModel.LocalPage)
                ViewModel.ShowLocalBeatmapsPage();
            else if (ViewModel.NavigationBeatmapsUserControl.ViewModel.OnlinePage)
                ViewModel.ShowOnlineBeatmapsPage();
            else
            {
                ViewModel.NavigationBeatmapsUserControl.ViewModel.LocalPage = true;
                ViewModel.ShowLocalBeatmapsPage();
            }
        }

        private void RadioButtonSabers_Checked(object sender, RoutedEventArgs e)
        {
            userControlNavigation.Content = ViewModel.NavigationSabersUserControl;

            if (ViewModel.NavigationSabersUserControl.ViewModel.LocalPage)
                ViewModel.ShowLocalModelSaberPage(ModelType.Saber);
            else if (ViewModel.NavigationSabersUserControl.ViewModel.OnlinePage)
                ViewModel.ShowOnlineModelSaberPage(ModelType.Saber);
            else
            {
                ViewModel.NavigationSabersUserControl.ViewModel.LocalPage = true;
                ViewModel.ShowLocalModelSaberPage(ModelType.Saber);
            }
        }

        private void RadioButtonAvatars_Checked(object sender, RoutedEventArgs e)
        {
            userControlNavigation.Content = ViewModel.NavigationAvatarsUserControl;

            if (ViewModel.NavigationAvatarsUserControl.ViewModel.LocalPage)
                ViewModel.ShowLocalModelSaberPage(ModelType.Avatar);
            else if (ViewModel.NavigationAvatarsUserControl.ViewModel.OnlinePage)
                ViewModel.ShowOnlineModelSaberPage(ModelType.Avatar);
            else
            {
                ViewModel.NavigationAvatarsUserControl.ViewModel.LocalPage = true;
                ViewModel.ShowLocalModelSaberPage(ModelType.Avatar);
            }
        }

        private void RadioButtonPlatforms_Checked(object sender, RoutedEventArgs e)
        {
            userControlNavigation.Content = ViewModel.NavigationPlatformsUserControl;

            if (ViewModel.NavigationPlatformsUserControl.ViewModel.LocalPage)
                ViewModel.ShowLocalModelSaberPage(ModelType.Platform);
            else if (ViewModel.NavigationPlatformsUserControl.ViewModel.OnlinePage)
                ViewModel.ShowOnlineModelSaberPage(ModelType.Platform);
            else
            {
                ViewModel.NavigationPlatformsUserControl.ViewModel.LocalPage = true;
                ViewModel.ShowLocalModelSaberPage(ModelType.Platform);
            }
        }

        private void RadioButtonBloqs_Checked(object sender, RoutedEventArgs e)
        {
            userControlNavigation.Content = ViewModel.NavigationBloqsUserControl;

            if (ViewModel.NavigationBloqsUserControl.ViewModel.LocalPage)
                ViewModel.ShowLocalModelSaberPage(ModelType.Bloq);
            else if (ViewModel.NavigationBloqsUserControl.ViewModel.OnlinePage)
                ViewModel.ShowOnlineModelSaberPage(ModelType.Bloq);
            else
            {
                ViewModel.NavigationBloqsUserControl.ViewModel.LocalPage = true;
                ViewModel.ShowLocalModelSaberPage(ModelType.Bloq);
            }
        }

        private void RadioButtonDownloads_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowDownloadsPage();
        }

        private void RadioButtonSettings_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowSettingsPage();
        }

        private async void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (ViewModel.BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps != null &&
                ViewModel.BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps.Maps.Any(x => x.IsDownloading))
            {
                e.Cancel = true;
                await this.ShowMessageAsync("Song(s) downloading", "You can't close the application while a song is downloading");
            }
            else
                Settings.CurrentSettings.Save();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            string beatSaverArg = args.FirstOrDefault(x => x.Contains("beatsaver://"));
            string modelSaberArg = args.FirstOrDefault(x => x.Contains("modelsaber://"));

            if (Settings.CurrentSettings.BeatSaver.OneClickInstaller && !string.IsNullOrEmpty(beatSaverArg))
            {
                string beatSaverKey = beatSaverArg.Substring(12).Replace("/", "");
                ViewModel.DownloadSong(beatSaverKey).ConfigureAwait(false);
            }
            if (Settings.CurrentSettings.ModelSaber.OneClickInstaller && !string.IsNullOrEmpty(modelSaberArg))
            {
                string modelSaberArgs = modelSaberArg.Substring(13);
                int typeSlash = modelSaberArgs.IndexOf("/");
                int idSlash = modelSaberArgs.IndexOf("/", typeSlash + 1);
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

                ModelType modelType = Enum.Parse<ModelType>(textInfo.ToTitleCase(modelSaberArgs.Substring(0, typeSlash)));
                int id = int.Parse(modelSaberArgs.Substring(typeSlash + 1, idSlash - (typeSlash + 1)));
                ViewModel.DownloadModel(id, modelType).ConfigureAwait(false);
            }

            ViewModel.IsLoaded = true;

            if (string.IsNullOrWhiteSpace(Settings.CurrentSettings.RootPath) || !Directory.Exists(Settings.CurrentSettings.RootPath) ||
                Settings.CurrentSettings.BeatSaver.OneClickInstaller && !ViewModel.SettingsUserControl.ViewModel.IsBeatSaverOneClick &&
                ViewModel.SettingsUserControl.ViewModel.IsRunningAsAdmin)
            {
                radioButtonSettings.IsChecked = true;
            }
            else
                radioButtonHome.IsChecked = true;
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Version version = await ViewModel.Updater.CheckForUpdatesAsync();

                if (version.IsCurrentVersion)
                    await this.ShowMessageAsync($"Up to date - {version}", "You're already using the latest version of the application");
            }
            catch (Exception ex)
            {
                if (string.Equals(ex.Message, ex.InnerException.Message))
                {
                    await this.ShowMessageAsync("Checking for updates failed", $"There was an error while checking for updates.\n\n" +
                                                                               $"Error:\n" +
                                                                               $"{ex.InnerException.Message}");
                }
                else
                {
                    await this.ShowMessageAsync("Checking for updates failed", $"There was an error while checking for updates.\n\n" +
                                                                               $"Error:\n" +
                                                                               $"{ex.Message} ({ex.InnerException.Message})");
                }
            }
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.XButton1)
            {
                if (userControlMain.Content == ViewModel.BeatmapLocalDetailsUserControl)
                    ViewModel.BeatmapLocalDetailsUserControl.ViewModel.Back();
                else if (userControlMain.Content == ViewModel.BeatmapOnlineDetailsUserControl)
                    ViewModel.BeatmapOnlineDetailsUserControl.ViewModel.Back();
            }
        }

        private void DoubleAnimation_CloseCover(object sender, EventArgs e)
        {
            ViewModel.CloseBigCover();
        }
    }
}
