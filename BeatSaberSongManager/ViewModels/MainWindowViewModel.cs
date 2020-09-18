using BeatSaberSongManager.UserControls;
using BeatSaverApi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberSongManager.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly MainWindow mainWindow;
        private bool localBeatmapsLoaded = false;

        public readonly MainWindowViewModel ViewModel;
        public readonly BeatmapLocalUserControl LocalUserControl;
        public readonly BeatmapOnlineUserControl OnlineUserControl;
        public readonly SettingsUserControl SettingsUserControl;
        public readonly BeatmapLocalDetailsUserControl LocalDetailsUserControl;

        public bool ShowLocalDetails;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            LocalUserControl = new BeatmapLocalUserControl(mainWindow);
            OnlineUserControl = new BeatmapOnlineUserControl(mainWindow);
            SettingsUserControl = new SettingsUserControl();
            LocalDetailsUserControl = new BeatmapLocalDetailsUserControl(mainWindow);
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

            mainWindow.transitionControl.Content = OnlineUserControl;
        }

        public void ShowSettingsPage()
        {
            mainWindow.transitionControl.Content = SettingsUserControl;
        }
    }
}
