using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using BeatSaverApi;
using BeatSaverApi.Events;
using MahApps.Metro.IconPacks;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace BeatSaberSongManager.ViewModels
{
    public class BeatMapOnlineUserControlViewModel : INotifyPropertyChanged
    {
        private readonly BeatmapOnlineUserControl userControl;
        private BeatSaverMaps beatSaverMaps;
        private Doc currentlyDownloading;

        public MapSort CurrentMapSort;
        public readonly MainWindow MainWindow;
        public readonly BeatSaver BeatSaverApi;

        public BeatSaverMaps BeatSaverMaps
        {
            get { return beatSaverMaps; }
            set
            {
                beatSaverMaps = value;
                OnPropertyChanged(nameof(BeatSaverMaps));
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
            e.Song.isDownloaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void GetBeatSaverMaps(MapSort mapSort, int page = 0)
        {
            MainWindow.progressRingLoading.IsActive = true;
            MainWindow.rectangleLoading.Visibility = Visibility.Visible;
            MainWindow.progressRingLoading.Visibility = Visibility.Visible;

            Thread thread = new Thread(async () => BeatSaverMaps = await BeatSaverApi.GetBeatSaverMaps(mapSort, page));
            thread.Start();
        }

        public void GetBeatSaverMaps(string query, int page = 0)
        {
            MainWindow.progressRingLoading.IsActive = true;
            MainWindow.rectangleLoading.Visibility = Visibility.Visible;
            MainWindow.progressRingLoading.Visibility = Visibility.Visible;

            Thread thread = new Thread(async () => BeatSaverMaps = await BeatSaverApi.GetBeatSaverMaps(query, page));
            thread.Start();
        }

        public void UpdatePageButtons()
        {
            if (BeatSaverMaps != null && BeatSaverMaps.prevPage.HasValue)
            {
                userControl.buttonFirstPage.IsEnabled = true;
                userControl.buttonPreviousPage.IsEnabled = true;
            }
            else
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
            }
            if (BeatSaverMaps != null && BeatSaverMaps.nextPage.HasValue)
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
                GetBeatSaverMaps(CurrentMapSort, BeatSaverMaps.nextPage.Value);
            else
                GetBeatSaverMaps(query, BeatSaverMaps.nextPage.Value);
        }

        public void PreviousPage(string query = null)
        {
            if (query is null)
                GetBeatSaverMaps(CurrentMapSort, BeatSaverMaps.prevPage.Value);
            else
                GetBeatSaverMaps(query, BeatSaverMaps.prevPage.Value);
        }

        public void FirstPage(string query = null)
        {
            if (query is null)
                GetBeatSaverMaps(CurrentMapSort, 0);
            else
                GetBeatSaverMaps(query, 0);
        }

        public void LastPage(string query = null)
        {
            if (query is null)
                GetBeatSaverMaps(CurrentMapSort, BeatSaverMaps.lastPage);
            else
                GetBeatSaverMaps(query, BeatSaverMaps.lastPage);
        }

        public void DownloadSong(string key)
        {
            Doc song = BeatSaverMaps.docs.FirstOrDefault(x => x.key == key);
            currentlyDownloading = song;
            BeatSaverApi.DownloadSong(song);
        }

        public void DeleteSong(string key)
        {
            Doc song = BeatSaverMaps.docs.FirstOrDefault(x => x.key == key);
            BeatSaverApi.DeleteSong(song);
        }
    }
}
