using BeatSaverApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberSongManager.ViewModels
{
    public class BeatMapOnlineUserControlViewModel : INotifyPropertyChanged
    {
        private BeatSaverMaps beatSaverMaps;

        public readonly BeatSaver BeatSaverApi;
        public BeatSaverMaps BeatSaverMaps
        {
            get { return beatSaverMaps; }
            set
            {
                if (value is null)
                    throw new NullReferenceException("The value can't be null");

                beatSaverMaps = value;
                OnPropertyChanged(nameof(BeatSaverMaps));
            }
        }

        public BeatMapOnlineUserControlViewModel()
        {
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
            Task.Run(async () =>
            {
                BeatSaverMaps = await BeatSaverApi.GetBeatSaverMaps(mapSort, page);
            });
        }

        //public async Task GetBeatSaverMaps(MapSort mapSort, int page = 0)
        //{
        //    BeatSaverMaps = await BeatSaverApi.GetBeatSaverMaps(mapSort, page);
        //}
    }
}
