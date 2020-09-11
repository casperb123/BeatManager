using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using BeatSaverApi;
using BeatSaverApi.Entities;
using BeatSaverApi.Events;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BeatSaberSongManager.ViewModels
{
    public class BeatMapOnlineUserControlViewModel : INotifyPropertyChanged
    {
        private readonly BeatmapOnlineUserControl userControl;
        private OnlineBeatmaps onlineBeatmaps;

        public MapSort CurrentMapSort;
        public readonly MainWindow MainWindow;
        public readonly BeatSaver BeatSaverApi;
        public bool SongDownloaded;

        public OnlineBeatmaps OnlineBeatmaps
        {
            get { return onlineBeatmaps; }
            set
            {
                onlineBeatmaps = value;
                OnPropertyChanged(nameof(OnlineBeatmaps));
            }
        }

        public BeatMapOnlineUserControlViewModel(MainWindow mainWindow, BeatmapOnlineUserControl userControl)
        {
            MainWindow = mainWindow;
            this.userControl = userControl;

            BeatSaverApi = new BeatSaver(Settings.CurrentSettings.SongsPath);
            BeatSaverApi.DownloadCompleted += BeatSaverApi_DownloadCompleted;
        }

        private void BeatSaverApi_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if (!OnlineBeatmaps.Maps.Any(x => x.IsDownloading))
            {
                MainWindow.radioButtonLocal.IsEnabled = true;
                MainWindow.radioButtonSettings.IsEnabled = true;
            }

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
            userControl.progressRingLoading.IsActive = true;
            userControl.rectangleLoading.Visibility = Visibility.Visible;
            userControl.progressRingLoading.Visibility = Visibility.Visible;

            _ = Task.Run(async () =>
            {
                OnlineBeatmaps = await BeatSaverApi.GetOnlineBeatmaps(mapSort, page);
                if (OnlineBeatmaps is null)
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

        public void GetBeatmaps(string query, int page = 0)
        {
            userControl.progressRingLoading.IsActive = true;
            userControl.rectangleLoading.Visibility = Visibility.Visible;
            userControl.progressRingLoading.Visibility = Visibility.Visible;

            _ = Task.Run(async () =>
            {
                OnlineBeatmaps = await BeatSaverApi.GetOnlineBeatmaps(query, page);
                if (OnlineBeatmaps is null)
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

        public async Task Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                await MainWindow.ShowMessageAsync("Beat Saver search", "The search query can't be null or empty");
                return;
            }
            if (!userControl.radioButtonSearch.IsChecked.Value)
                userControl.radioButtonSearch.IsChecked = true;

            GetBeatmaps(query);
        }

        public void UpdatePageButtons()
        {
            if (OnlineBeatmaps is null || OnlineBeatmaps.Maps.Any(x => x.IsDownloading))
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

        public void DownloadSong(string key)
        {
            MainWindow.radioButtonLocal.IsEnabled = false;
            MainWindow.radioButtonSettings.IsEnabled = false;
            OnlineBeatmap song = OnlineBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
            BeatSaverApi.DownloadSong(song).ConfigureAwait(false);
            SongDownloaded = true;
        }

        public void DeleteSong(string key)
        {
            OnlineBeatmap song = OnlineBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
            BeatSaverApi.DeleteSong(song);
        }
    }
}
