using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using BeatSaberSongManager.ViewModels;
using BeatSaverApi.Entities;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeatSaberSongManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly BeatmapLocalUserControl localUserControl;
        private readonly BeatmapOnlineUserControl onlineUserControl;
        private readonly SettingsUserControl settingsUserControl;
        private bool localBeatmapsLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            localUserControl = new BeatmapLocalUserControl(this);
            onlineUserControl = new BeatmapOnlineUserControl(this);
            settingsUserControl = new SettingsUserControl(this);
        }

        private void RadioButtonLocal_Checked(object sender, RoutedEventArgs e)
        {
            userControlMain.Content = localUserControl;
            if (!localBeatmapsLoaded ||
                onlineUserControl.ViewModel.SongDownloaded ||
                settingsUserControl.ViewModel.SongsPathChanged)
            {
                localBeatmapsLoaded = true;
                onlineUserControl.ViewModel.SongDownloaded = false;
                settingsUserControl.ViewModel.SongsPathChanged = false;
                localUserControl.ViewModel.GetBeatmaps();
            }
        }

        private void RadioButtonOnline_Checked(object sender, RoutedEventArgs e)
        {
            if (localUserControl.ViewModel.SongDeleted)
            {
                if (onlineUserControl.ViewModel.OnlineBeatmaps != null)
                {
                    MapSort mapSort = onlineUserControl.ViewModel.CurrentMapSort;
                    if (mapSort == MapSort.Search)
                        onlineUserControl.ViewModel.GetBeatmaps(onlineUserControl.textBoxSearch.Text, onlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                    else
                        onlineUserControl.ViewModel.GetBeatmaps(mapSort, onlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                }

                localUserControl.ViewModel.SongDeleted = false;
            }

            userControlMain.Content = onlineUserControl;
        }

        private void RadioButtonSettings_Checked(object sender, RoutedEventArgs e)
        {
            userControlMain.Content = settingsUserControl;
        }

        private async void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (onlineUserControl.ViewModel.OnlineBeatmaps != null && onlineUserControl.ViewModel.OnlineBeatmaps.Maps.Any(x => x.IsDownloading))
            {
                e.Cancel = true;
                await this.ShowMessageAsync("Song(s) downloading", "You can't close the application while a song is downloading");
            }
            else
                Settings.CurrentSettings.Save();
        }
    }
}
