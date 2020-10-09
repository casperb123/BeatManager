using BeatSaverApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BeatManager.ViewModels.Download
{
    public class OnlineBeatmapCompletedUserControlViewModel : INotifyPropertyChanged
    {
        private OnlineBeatmap beatmap;
        private string downloaded;

        public string Downloaded
        {
            get { return downloaded; }
            set
            {
                downloaded = value;
                OnPropertyChanged(nameof(Downloaded));
            }
        }

        public OnlineBeatmap Beatmap
        {
            get { return beatmap; }
            set
            {
                beatmap = value;
                OnPropertyChanged(nameof(Beatmap));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public OnlineBeatmapCompletedUserControlViewModel(OnlineBeatmap beatmap)
        {
            Beatmap = beatmap;
        }
    }
}
