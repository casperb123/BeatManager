using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using BeatSaverApi;
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
        private OnlineBeatMaps onlineBeatmaps;

        public MapSort CurrentMapSort;
        public readonly MainWindow MainWindow;
        public readonly BeatSaver BeatSaverApi;

        public OnlineBeatMaps OnlineBeatmaps
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
            BeatSaverApi.DownloadCompleted += (s, e) => UpdatePageButtons();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void GetBeatmaps(MapSort mapSort, int page = 0)
        {
            MainWindow.progressRingLoading.IsActive = true;
            MainWindow.rectangleLoading.Visibility = Visibility.Visible;
            MainWindow.progressRingLoading.Visibility = Visibility.Visible;

            _ = Task.Run(async () => OnlineBeatmaps = await BeatSaverApi.GetOnlineBeatmaps(mapSort, page));
        }

        public void GetBeatmaps(string query, int page = 0)
        {
            MainWindow.progressRingLoading.IsActive = true;
            MainWindow.rectangleLoading.Visibility = Visibility.Visible;
            MainWindow.progressRingLoading.Visibility = Visibility.Visible;

            _ = Task.Run(async () => OnlineBeatmaps = await BeatSaverApi.GetOnlineBeatmaps(query, page));
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
            OnlineBeatMap song = OnlineBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
            BeatSaverApi.DownloadSong(song).ConfigureAwait(false);
        }

        public void DeleteSong(string key)
        {
            OnlineBeatMap song = OnlineBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
            BeatSaverApi.DeleteSong(song);
        }
    }
}
