﻿using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using BeatSaverApi;
using BeatSaverApi.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BeatSaberSongManager.ViewModels
{
    public class BeatmapLocalUserControlViewModel : INotifyPropertyChanged
    {
        private LocalBeatmaps localBeatmaps;
        private readonly BeatmapLocalUserControl userControl;
        private readonly BeatSaver beatSaverApi;
        private int selectedSongsCount;

        public readonly MainWindow MainWindow;
        public bool SongDeleted;

        public LocalBeatmaps LocalBeatmaps
        {
            get { return localBeatmaps; }
            set
            {
                localBeatmaps = value;
                OnPropertyChanged(nameof(LocalBeatmaps));
            }
        }
        public List<LocalBeatmap> SelectedSongs { get; set; }
        public int SelectedSongsCount
        {
            get { return selectedSongsCount; }
            set
            {
                selectedSongsCount = value;
                OnPropertyChanged(nameof(SelectedSongsCount));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public BeatmapLocalUserControlViewModel(BeatmapLocalUserControl userControl, MainWindow mainWindow)
        {
            this.userControl = userControl;
            MainWindow = mainWindow;
            beatSaverApi = new BeatSaver(Settings.CurrentSettings.CustomLevelsPath);
            SelectedSongs = new List<LocalBeatmap>();
        }

        public void GetBeatmaps()
        {
            MainWindow.progressRingLoading.IsActive = true;
            MainWindow.rectangleLoading.Visibility = Visibility.Visible;
            MainWindow.progressRingLoading.Visibility = Visibility.Visible;

            _ = Task.Run(async () => LocalBeatmaps = await App.BeatSaverApi.GetLocalBeatmaps());
        }

        public void GetBeatmaps(LocalBeatmaps localBeatmaps)
        {
            MainWindow.progressRingLoading.IsActive = true;
            MainWindow.rectangleLoading.Visibility = Visibility.Visible;
            MainWindow.progressRingLoading.Visibility = Visibility.Visible;

            _ = Task.Run(async () => LocalBeatmaps = await App.BeatSaverApi.GetLocalBeatmaps(localBeatmaps));
        }

        public void UpdatePageButtons()
        {
            if (LocalBeatmaps is null)
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
                userControl.buttonLastPage.IsEnabled = false;
                userControl.buttonNextPage.IsEnabled = false;
                return;
            }

            if (LocalBeatmaps != null && LocalBeatmaps.PrevPage.HasValue)
            {
                userControl.buttonFirstPage.IsEnabled = true;
                userControl.buttonPreviousPage.IsEnabled = true;
            }
            else
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
            }
            if (LocalBeatmaps != null && LocalBeatmaps.NextPage.HasValue)
            {
                userControl.buttonLastPage.IsEnabled = true;
                userControl.buttonNextPage.IsEnabled = true;
            }
            else
            {
                userControl.buttonLastPage.IsEnabled = false;
                userControl.buttonNextPage.IsEnabled = false;
            }
        }

        public void NextPage()
        {
            LocalBeatmaps = beatSaverApi.ChangeLocalPage(LocalBeatmaps, LocalBeatmaps.NextPage.Value);
        }

        public void PreviousPage()
        {
            LocalBeatmaps = beatSaverApi.ChangeLocalPage(LocalBeatmaps, LocalBeatmaps.PrevPage.Value);
        }

        public void FirstPage()
        {
            LocalBeatmaps = beatSaverApi.ChangeLocalPage(LocalBeatmaps, 0);
        }

        public void LastPage()
        {
            LocalBeatmaps = beatSaverApi.ChangeLocalPage(LocalBeatmaps, LocalBeatmaps.LastPage);
        }

        public void DeleteSong(string key)
        {
            LocalBeatmap localBeatmap = LocalBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
            OnlineBeatmap onlineBeatmap = MainWindow.ViewModel.OnlineUserControl.ViewModel.OnlineBeatmaps?.Maps.FirstOrDefault(x => x.Key == key);

            beatSaverApi.DeleteSong(localBeatmap);
            LocalBeatmaps.Maps.Remove(localBeatmap);
            if (onlineBeatmap is null)
                SongDeleted = true;
            else
                onlineBeatmap.IsDownloaded = false;

            LocalBeatmaps = beatSaverApi.RefreshLocalPages(LocalBeatmaps);
        }

        public void DeleteSongs(List<LocalBeatmap> songs)
        {
            beatSaverApi.DeleteSongs(songs);
            songs.ForEach(x => LocalBeatmaps.Maps.Remove(x));
            List<OnlineBeatmap> onlineBeatmaps = new List<OnlineBeatmap>();
            foreach (LocalBeatmap localBeatmap in songs)
            {
                OnlineBeatmap onlineBeatmap = MainWindow.ViewModel.OnlineUserControl.ViewModel.OnlineBeatmaps?.Maps.FirstOrDefault(x => x.Key == localBeatmap.Key);
                if (onlineBeatmap != null)
                    onlineBeatmaps.Add(onlineBeatmap);
            }

            if (onlineBeatmaps.Count == songs.Count)
                onlineBeatmaps.ForEach(x => x.IsDownloaded = false);
            else
                SongDeleted = true;

            LocalBeatmaps = beatSaverApi.RefreshLocalPages(LocalBeatmaps);
        }

        public void BeatmapDetails(string key)
        {
            MainWindow.ViewModel.ShowLocalDetails = true;
            LocalBeatmap beatmap = LocalBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
            MainWindow.ViewModel.LocalDetailsUserControl.ViewModel.Beatmap = beatmap;
            MainWindow.transitionControl.Content = MainWindow.ViewModel.LocalDetailsUserControl;
            MainWindow.ViewModel.LocalUserControl.dataGridMaps.UnselectAll();
        }
    }
}
