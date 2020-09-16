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
        public readonly BeatmapLocalUserControl LocalUserControl;
        public readonly BeatmapOnlineUserControl OnlineUserControl;
        public readonly SettingsUserControl SettingsUserControl;
        public readonly BeatmapLocalDetailsUserControl LocalDetailsUserControl;
        private bool localBeatmapsLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            LocalUserControl = new BeatmapLocalUserControl(this);
            OnlineUserControl = new BeatmapOnlineUserControl(this);
            SettingsUserControl = new SettingsUserControl(this);
            LocalDetailsUserControl = new BeatmapLocalDetailsUserControl(this);
        }

        private void RadioButtonLocal_Checked(object sender, RoutedEventArgs e)
        {
            userControlMain.Content = LocalUserControl;
            if (!localBeatmapsLoaded ||
                OnlineUserControl.ViewModel.SongChanged ||
                SettingsUserControl.ViewModel.SongsPathChanged)
            {
                localBeatmapsLoaded = true;
                OnlineUserControl.ViewModel.SongChanged = false;
                SettingsUserControl.ViewModel.SongsPathChanged = false;
                if (LocalUserControl.ViewModel.LocalBeatmaps is null)
                    LocalUserControl.ViewModel.GetBeatmaps();
                else
                    LocalUserControl.ViewModel.GetBeatmaps(LocalUserControl.ViewModel.LocalBeatmaps);
            }
        }

        private void RadioButtonOnline_Checked(object sender, RoutedEventArgs e)
        {
            if (LocalUserControl.ViewModel.SongDeleted)
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

            userControlMain.Content = OnlineUserControl;
        }

        private void RadioButtonSettings_Checked(object sender, RoutedEventArgs e)
        {
            userControlMain.Content = SettingsUserControl;
        }

        private async void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (OnlineUserControl.ViewModel.OnlineBeatmaps != null && OnlineUserControl.ViewModel.OnlineBeatmaps.Maps.Any(x => x.IsDownloading))
            {
                e.Cancel = true;
                await this.ShowMessageAsync("Song(s) downloading", "You can't close the application while a song is downloading");
            }
            else
                Settings.CurrentSettings.Save();
        }
    }
}
