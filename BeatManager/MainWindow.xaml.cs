using BeatManager.Entities;
using BeatManager.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Octokit;
using System;
using System.ComponentModel;
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
        }

        private async void BeatSaverPipe_OnRequest(string key)
        {
            await ViewModel.DownloadSong(key);
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
                ViewModel.ShowLocalPage();
            else if (ViewModel.NavigationBeatmapsUserControl.ViewModel.OnlinePage)
                ViewModel.ShowOnlinePage();
            else
            {
                ViewModel.NavigationBeatmapsUserControl.ViewModel.LocalPage = true;
                ViewModel.ShowLocalPage();
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
            if (ViewModel.OnlineUserControl.ViewModel.OnlineBeatmaps != null &&
                ViewModel.OnlineUserControl.ViewModel.OnlineBeatmaps.Maps.Any(x => x.IsDownloading))
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

            if (!string.IsNullOrEmpty(beatSaverArg))
            {
                string beatSaverKey = beatSaverArg.Substring(12).Replace("/", "");
                ViewModel.DownloadSong(beatSaverKey).ConfigureAwait(false);
            }

            ViewModel.IsLoaded = true;

            if (string.IsNullOrWhiteSpace(Settings.CurrentSettings.RootPath) || !Directory.Exists(Settings.CurrentSettings.RootPath) ||
                Settings.CurrentSettings.BeatSaverOneClickInstaller && !ViewModel.SettingsUserControl.ViewModel.IsBeatSaverOneClick &&
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
                if (userControlMain.Content == ViewModel.LocalDetailsUserControl)
                    ViewModel.LocalDetailsUserControl.ViewModel.Back();
                else if (userControlMain.Content == ViewModel.OnlineDetailsUserControl)
                    ViewModel.OnlineDetailsUserControl.ViewModel.Back();
            }
        }
    }
}
