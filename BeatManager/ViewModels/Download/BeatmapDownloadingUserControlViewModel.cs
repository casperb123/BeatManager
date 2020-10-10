using BeatSaverApi.Entities;
using BeatSaverApi.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BeatManager.ViewModels.Download
{
    public class BeatmapDownloadingUserControlViewModel : INotifyPropertyChanged
    {
        private OnlineBeatmap beatmap;
        private string downloadTimeLeft;
        private string downloadTimeSpent;
        private int progressPercent;

        public string ToDownload { get; set; }

        public int ProgressPercent
        {
            get { return progressPercent; }
            set
            {
                progressPercent = value;
                OnPropertyChanged(nameof(ProgressPercent));
            }
        }

        public string DownloadTimeSpent
        {
            get { return downloadTimeSpent; }
            set
            {
                downloadTimeSpent = value;
                OnPropertyChanged(nameof(DownloadTimeSpent));
            }
        }

        public string DownloadTimeLeft
        {
            get { return downloadTimeLeft; }
            set
            {
                downloadTimeLeft = value;
                OnPropertyChanged(nameof(DownloadTimeLeft));
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

        public BeatmapDownloadingUserControlViewModel(OnlineBeatmap beatmap)
        {
            Beatmap = beatmap;
        }
    }
}
