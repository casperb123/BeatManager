using BeatSaberSongManager.Entities;
using BeatSaberSongManager.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Octokit;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GitHubUpdater;
using Version = GitHubUpdater.Version;

namespace BeatSaberSongManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public readonly MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel(this);
            DataContext = ViewModel;
        }

        private void RadioButtonLocal_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowLocalPage();
        }

        private void RadioButtonOnline_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowOnlinePage();
        }

        private void RadioButtonSettings_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowSettingsPage();
        }


        private void TransitionControl_TransitionCompleted(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.ShowLocalDetails)
                ViewModel.LocalDetailsUserControl.scrollViewer.ScrollToTop();
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
            radioButtonLocal.IsChecked = true;
            ViewModel.ShowLocalPage();
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Version version = await ViewModel.Updater.CheckForUpdatesAsync();

                if (version.IsCurrentVersion)
                    await this.ShowMessageAsync($"Up to date - {version}", "You're already using the latest version of the application");
            }
            catch (ApiException ex)
            {
                if (ex.InnerException is null)
                    await this.ShowMessageAsync("Checking for updates failed", $"There was an error while checking for updates.\n\n" +
                                                                               $"Error:\n" +
                                                                               $"{ex.Message}");
                else
                    await this.ShowMessageAsync("Checking for updates failed", $"There was an error while checking for updates.\n\n" +
                                                                               $"Error:\n" +
                                                                               $"{ex.InnerException.Message}");
            }
        }
    }
}
