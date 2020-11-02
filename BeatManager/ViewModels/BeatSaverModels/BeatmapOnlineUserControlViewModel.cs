using BeatManager.UserControls.BeatSaver;
using BeatSaver.Entities;
using BeatSaver.Events;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BeatManager.ViewModels.BeatSaverModels
{
    public class BeatmapOnlineUserControlViewModel : INotifyPropertyChanged
    {
        private readonly BeatmapOnlineUserControl userControl;
        private OnlineBeatmaps onlineBeatmaps;

        private int selectedSongsToDownloadCount;
        private int selectedSongsToDeleteCount;
        private MapSort currentMapSort;
        private string searchWatermark;

        public readonly MainWindow MainWindow;
        public bool SongChanged;
        public bool IsLoaded;

        public MapSort CurrentMapSort
        {
            get { return currentMapSort; }
            set
            {
                currentMapSort = value;
                OnPropertyChanged(nameof(CurrentMapSort));

                if (value == MapSort.SearchKey)
                    SearchWatermark = "Search by key";
                else if (value == MapSort.SearchHash)
                    SearchWatermark = "Search by hash";
                else
                    SearchWatermark = "Search by name, description or uploader";
            }
        }
        public string SearchWatermark
        {
            get { return searchWatermark; }
            set
            {
                searchWatermark = value;
                OnPropertyChanged(nameof(SearchWatermark));
            }
        }

        public string CurrentSearchQuery { get; private set; }

        public List<OnlineBeatmap> SelectedSongs { get; set; }
        public int SelectedSongsToDeleteCount
        {
            get { return selectedSongsToDeleteCount; }
            set
            {
                selectedSongsToDeleteCount = value;
                OnPropertyChanged(nameof(SelectedSongsToDeleteCount));
            }
        }
        public int SelectedSongsToDownloadCount
        {
            get { return selectedSongsToDownloadCount; }
            set
            {
                selectedSongsToDownloadCount = value;
                OnPropertyChanged(nameof(SelectedSongsToDownloadCount));
            }
        }
        public OnlineBeatmaps OnlineBeatmaps
        {
            get { return onlineBeatmaps; }
            set
            {
                onlineBeatmaps = value;
                OnPropertyChanged(nameof(OnlineBeatmaps));
            }
        }

        public BeatmapOnlineUserControlViewModel(MainWindow mainWindow, BeatmapOnlineUserControl userControl)
        {
            MainWindow = mainWindow;
            this.userControl = userControl;
            SelectedSongs = new List<OnlineBeatmap>();

            App.BeatSaverApi.DownloadCompleted += BeatSaverApi_DownloadCompleted;
        }

        private void BeatSaverApi_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if (OnlineBeatmaps is null)
                return;

            if (!OnlineBeatmaps.Maps.Contains(e.Beatmap))
            {
                OnlineBeatmap onlineBeatmap = OnlineBeatmaps.Maps.FirstOrDefault(x => x.Key == e.Beatmap.Key);
                if (onlineBeatmap != null)
                {
                    onlineBeatmap.IsDownloading = e.Beatmap.IsDownloading;
                    onlineBeatmap.IsDownloaded = e.Beatmap.IsDownloaded;
                }
            }

            if (!OnlineBeatmaps.Maps.Any(x => x.IsDownloading) && !App.BeatSaverApi.Downloading.Any())
                MainWindow.radioButtonSettings.IsEnabled = true;

            MainWindow.ViewModel.OnlineBeatmapChanged = true;
            UpdatePageButtons();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void GetBeatmaps(MapSort mapSort, int page = 0)
        {
            MainWindow.ToggleLoading(true, "Loading online beatmaps", $"Sorting by: {mapSort}");

            _ = Task.Run(async () =>
            {
                try
                {
                    OnlineBeatmaps = await App.BeatSaverApi.GetOnlineBeatmaps(mapSort, page);
                }
                catch (Exception e)
                {
                    string description = e.Message;
                    if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                        description += $" ({e.InnerException.Message})";

                    await MainWindow.ShowMessageAsync("Online Beatmaps", description);
                    userControl.radioButtonSearch.IsChecked = false;
                    userControl.radioButtonHot.IsChecked = false;
                    userControl.radioButtonRating.IsChecked = false;
                    userControl.radioButtonLatest.IsChecked = false;
                    userControl.radioButtonDownloads.IsChecked = false;
                    userControl.radioButtonPlays.IsChecked = false;
                }
            });
        }

        public void GetBeatmaps(string query, int page = 0)
        {
            string description = null;
            if (CurrentMapSort == MapSort.Search)
                description = $"Searching by name: {query}";
            else if (CurrentMapSort == MapSort.SearchKey)
                description = $"Searching by key: {query}";
            else if (CurrentMapSort == MapSort.SearchHash)
                description = $"Searching by hash: {query}";

            MainWindow.ToggleLoading(true, "Loading online beatmaps", description);

            _ = Task.Run(async () =>
            {
                (bool isValid, OnlineBeatmaps onlineBeatmaps) = await App.BeatSaverApi.GetOnlineBeatmaps(query, CurrentMapSort, page);
                if (isValid)
                    OnlineBeatmaps = onlineBeatmaps;
                else
                {
                    await MainWindow.Dispatcher.Invoke(async () =>
                    {
                        await MainWindow.ShowMessageAsync("Can't connect to BeatSaver", "Either you don't have any internet connection or BeatSaver is currently offline");
                        userControl.radioButtonSearch.IsChecked = false;
                        userControl.radioButtonHot.IsChecked = false;
                        userControl.radioButtonRating.IsChecked = false;
                        userControl.radioButtonLatest.IsChecked = false;
                        userControl.radioButtonDownloads.IsChecked = false;
                        userControl.radioButtonPlays.IsChecked = false;
                    });
                }
            });
        }

        public async void Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return;

            if (OnlineBeatmaps != null && OnlineBeatmaps.Maps.Any(x => x.IsDownloading))
            {
                await MainWindow.ShowMessageAsync("Beat Saver search", "You can't search while a song is downloading");
                return;
            }

            if (!userControl.radioButtonSearch.IsChecked.Value &&
                CurrentMapSort != MapSort.SearchKey &&
                CurrentMapSort != MapSort.SearchHash)
            {
                userControl.radioButtonSearch.IsChecked = true;
            }

            CurrentSearchQuery = query;
            GetBeatmaps(query);
        }

        public void UpdatePageButtons()
        {
            if (OnlineBeatmaps is null)
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
                userControl.buttonLastPage.IsEnabled = false;
                userControl.buttonNextPage.IsEnabled = false;
                return;
            }

            if (OnlineBeatmaps != null && OnlineBeatmaps.PrevPage.HasValue)
            {
                userControl.buttonFirstPage.IsEnabled = true;
                userControl.buttonPreviousPage.IsEnabled = true;
            }
            else
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
            }
            if (OnlineBeatmaps != null && OnlineBeatmaps.NextPage.HasValue)
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

        public void NextPage(string query = null)
        {
            if (query is null)
                GetBeatmaps(CurrentMapSort, OnlineBeatmaps.NextPage.Value);
            else
                GetBeatmaps(query, OnlineBeatmaps.NextPage.Value);
        }

        public void PreviousPage(string query = null)
        {
            if (query is null)
                GetBeatmaps(CurrentMapSort, OnlineBeatmaps.PrevPage.Value);
            else
                GetBeatmaps(query, OnlineBeatmaps.PrevPage.Value);
        }

        public void FirstPage(string query = null)
        {
            if (query is null)
                GetBeatmaps(CurrentMapSort, 0);
            else
                GetBeatmaps(query, 0);
        }

        public void LastPage(string query = null)
        {
            if (query is null)
                GetBeatmaps(CurrentMapSort, OnlineBeatmaps.LastPage);
            else
                GetBeatmaps(query, OnlineBeatmaps.LastPage);
        }

        public async void DownloadSong(string key)
        {
            try
            {
                MainWindow.radioButtonSettings.IsEnabled = false;
                OnlineBeatmap song = OnlineBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
                await App.BeatSaverApi.DownloadSong(song);
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
                    DownloadSong(key);
            }
        }

        public async void DownloadSongs(List<OnlineBeatmap> songs)
        {
            MainWindow.radioButtonSettings.IsEnabled = false;

            foreach (OnlineBeatmap beatmap in songs)
            {
                try
                {
                    _ = App.BeatSaverApi.DownloadSong(beatmap).ConfigureAwait(false);
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
                        DownloadSong(beatmap.Key);
                }
            }
        }

        public void DeleteSong(string key)
        {
            OnlineBeatmap onlineBeatmap = OnlineBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
            LocalBeatmap localBeatmap = MainWindow.ViewModel.BeatmapLocalUserControl.ViewModel.LocalBeatmaps?.Maps.FirstOrDefault(x => x.Identifier.Value == onlineBeatmap.Key || x.Identifier.Value == onlineBeatmap.Hash);

            if (localBeatmap != null)
                MainWindow.ViewModel.BeatmapLocalUserControl.ViewModel.LocalBeatmaps.Maps.Remove(localBeatmap);

            App.BeatSaverApi.DeleteSong(onlineBeatmap);
            MainWindow.ViewModel.OnlineBeatmapChanged = true;
        }

        public void DeleteSongs(ICollection<OnlineBeatmap> songs)
        {
            foreach (OnlineBeatmap onlineBeatmap in songs)
            {
                LocalBeatmap localBeatmap = MainWindow.ViewModel.BeatmapLocalUserControl.ViewModel.LocalBeatmaps?.Maps.FirstOrDefault(x => x.Identifier.Value == onlineBeatmap.Key || x.Identifier.Value == onlineBeatmap.Hash);

                if (localBeatmap != null)
                    MainWindow.ViewModel.BeatmapLocalUserControl.ViewModel.LocalBeatmaps.Maps.Remove(localBeatmap);

                App.BeatSaverApi.DeleteSong(onlineBeatmap);
            }

            if (songs.Count > 0)
                MainWindow.ViewModel.OnlineBeatmapChanged = true;
        }

        public void BeatmapDetails(string key, bool changePage = true)
        {
            MainWindow.ViewModel.ShowOnlineBeatmapDetails = true;
            OnlineBeatmap beatmap = OnlineBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
            MainWindow.ViewModel.BeatmapOnlineDetailsUserControl.ViewModel.Beatmap = beatmap;
            if (changePage)
                MainWindow.userControlMain.Content = MainWindow.ViewModel.BeatmapOnlineDetailsUserControl;

            MainWindow.ViewModel.BeatmapOnlineUserControl.dataGridMaps.UnselectAll();
        }
    }
}
