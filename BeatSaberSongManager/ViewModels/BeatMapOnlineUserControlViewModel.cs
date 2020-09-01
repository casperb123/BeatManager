using BeatSaberSongManager.UserControls;
using BeatSaverApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BeatSaberSongManager.ViewModels
{
    public class BeatMapOnlineUserControlViewModel : INotifyPropertyChanged
    {
        private BeatSaverMaps beatSaverMaps;
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

        public BeatMapOnlineUserControlViewModel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            BeatSaverApi = new BeatSaver();
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
    }
}
