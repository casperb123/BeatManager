﻿using BeatManager.UserControls.BeatSaver;
using BeatSaver.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BeatManager.ViewModels.BeatSaverModels
{
    public class BeatmapLocalUserControlViewModel : INotifyPropertyChanged
    {
        private LocalBeatmaps localBeatmaps;
        private LocalBeatmap beatmap;
        private readonly BeatmapLocalUserControl userControl;
        private int selectedSongsCount;

        public readonly MainWindow MainWindow;
        public bool SongDeleted;
        public bool IsLoaded;

        public LocalBeatmap Beatmap
        {
            get { return beatmap; }
            set
            {
                beatmap = value;
                OnPropertyChanged(nameof(Beatmap));
            }
        }

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
            SelectedSongs = new List<LocalBeatmap>();
        }

        public void GetBeatmaps()
        {
            MainWindow.ToggleLoading(true, "Loading local beatmaps");
            _ = Task.Run(async () => LocalBeatmaps = await App.BeatSaverApi.GetLocalBeatmaps());
        }

        public void GetBeatmaps(LocalBeatmaps localBeatmaps)
        {
            MainWindow.ToggleLoading(true, "Loading local beatmaps");
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
            App.BeatSaverApi.ChangeLocalPage(LocalBeatmaps, LocalBeatmaps.NextPage.Value);
            UpdateBeatmaps();
        }

        public void PreviousPage()
        {
            App.BeatSaverApi.ChangeLocalPage(LocalBeatmaps, LocalBeatmaps.PrevPage.Value);
            UpdateBeatmaps();
        }

        public void FirstPage()
        {
            App.BeatSaverApi.ChangeLocalPage(LocalBeatmaps, 0);
            UpdateBeatmaps();
        }

        public void LastPage()
        {
            App.BeatSaverApi.ChangeLocalPage(LocalBeatmaps, LocalBeatmaps.LastPage);
            UpdateBeatmaps();
        }

        private void UpdateBeatmaps()
        {
            userControl.dataGridMaps.UnselectAll();
            userControl.dataGridMaps.Items.Refresh();
            UpdatePageButtons();
        }

        public void DeleteSong(LocalIdentifier identifier)
        {
            LocalBeatmap localBeatmap = LocalBeatmaps.Maps.FirstOrDefault(x => x.Identifier.Value == identifier.Value);
            OnlineBeatmap onlineBeatmap;
            if (identifier.IsKey)
                onlineBeatmap = MainWindow.ViewModel.BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps?.Maps.FirstOrDefault(x => x.Key == identifier.Value);
            else
                onlineBeatmap = MainWindow.ViewModel.BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps?.Maps.FirstOrDefault(x => x.Hash == identifier.Value);

            App.BeatSaverApi.DeleteSong(localBeatmap);
            LocalBeatmaps.Maps.Remove(localBeatmap);
            if (onlineBeatmap is null)
                MainWindow.ViewModel.LocalBeatmapChanged = true;
            else
                onlineBeatmap.IsDownloaded = false;

            LocalBeatmaps = App.BeatSaverApi.RefreshLocalPages(LocalBeatmaps);
        }

        public void DeleteSongs(List<LocalBeatmap> songs)
        {
            foreach (LocalBeatmap localBeatmap in songs)
            {
                LocalBeatmaps.Maps.Remove(localBeatmap);
                App.BeatSaverApi.DeleteSong(localBeatmap);
            }

            List<OnlineBeatmap> onlineBeatmaps = new List<OnlineBeatmap>();
            foreach (LocalBeatmap localBeatmap in songs)
            {
                OnlineBeatmap onlineBeatmap;
                if (localBeatmap.Identifier.IsKey)
                    onlineBeatmap = MainWindow.ViewModel.BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps?.Maps.FirstOrDefault(x => x.Key == localBeatmap.Identifier.Value);
                else
                    onlineBeatmap = MainWindow.ViewModel.BeatmapOnlineUserControl.ViewModel.OnlineBeatmaps?.Maps.FirstOrDefault(x => x.Hash == localBeatmap.Identifier.Value);

                if (onlineBeatmap != null)
                    onlineBeatmaps.Add(onlineBeatmap);
            }

            if (onlineBeatmaps.Count == songs.Count)
                onlineBeatmaps.ForEach(x => x.IsDownloaded = false);
            else
                MainWindow.ViewModel.LocalBeatmapChanged = true;

            LocalBeatmaps = App.BeatSaverApi.RefreshLocalPages(LocalBeatmaps);
        }

        public void BeatmapDetails(LocalIdentifier identifier, bool changePage = true)
        {
            MainWindow.ViewModel.ShowLocalBeatmapDetails = true;
            LocalBeatmap beatmap = LocalBeatmaps.Maps.FirstOrDefault(x => x.Identifier.Value == identifier.Value);
            MainWindow.ViewModel.BeatmapLocalDetailsUserControl.ViewModel.Beatmap = beatmap;

            if (changePage)
                MainWindow.userControlMain.Content = MainWindow.ViewModel.BeatmapLocalDetailsUserControl;

            MainWindow.ViewModel.BeatmapLocalUserControl.dataGridMaps.UnselectAll();
        }

        public void OpenBigCover()
        {
            userControl.gridCoverImage.Visibility = Visibility.Visible;
        }

        public void CloseBigCover()
        {
            userControl.gridCoverImage.Visibility = Visibility.Hidden;
            userControl.gridCoverImage.Opacity = 0;
        }
    }
}