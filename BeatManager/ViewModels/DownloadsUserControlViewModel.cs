using BeatManager.UserControls.Download;
using BeatSaverApi.Entities;
using BeatSaverApi.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BeatManager.ViewModels
{
    public class DownloadsUserControlViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<OnlineBeatmapDownloadingUserControl> downloading;
        private ObservableCollection<OnlineBeatmapCompletedUserControl> completed;
        private int downloadingCount;
        private int completedCount;

        public int CompletedCount
        {
            get { return completedCount; }
            set
            {
                completedCount = value;
                OnPropertyChanged(nameof(CompletedCount));
            }
        }

        public int DownloadingCount
        {
            get { return downloadingCount; }
            set
            {
                downloadingCount = value;
                OnPropertyChanged(nameof(DownloadingCount));
            }
        }

        public ObservableCollection<OnlineBeatmapCompletedUserControl> Completed
        {
            get { return completed; }
            set
            {
                completed = value;
                OnPropertyChanged(nameof(Completed));
            }
        }

        public ObservableCollection<OnlineBeatmapDownloadingUserControl> Downloading
        {
            get { return downloading; }
            set
            {
                downloading = value;
                OnPropertyChanged(nameof(Downloading));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public DownloadsUserControlViewModel()
        {
            downloading = new ObservableCollection<OnlineBeatmapDownloadingUserControl>();
            completed = new ObservableCollection<OnlineBeatmapCompletedUserControl>();

            App.BeatSaverApi.DownloadStarted += BeatSaverApi_DownloadStarted;
            App.BeatSaverApi.DownloadProgressed += BeatSaverApi_DownloadProgressed;
            App.BeatSaverApi.DownloadCompleted += BeatSaverApi_DownloadCompleted;
        }

        private void BeatSaverApi_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            OnlineBeatmapDownloadingUserControl downloadingUserControl = Downloading.FirstOrDefault(x => x.ViewModel.Beatmap == e.Song);
            if (downloadingUserControl is null)
                return;

            Downloading.Remove(downloadingUserControl);
            OnlineBeatmapCompletedUserControl completedUserControl = new OnlineBeatmapCompletedUserControl(e.Song, downloadingUserControl.ViewModel.ToDownload);
            Completed.Add(completedUserControl);
        }

        private void BeatSaverApi_DownloadProgressed(object sender, DownloadProgressedEventArgs e)
        {
            OnlineBeatmapDownloadingUserControl userControl = Downloading.FirstOrDefault(x => x.ViewModel.Beatmap == e.Beatmap);
            if (userControl is null)
                return;

            userControl.ViewModel.ToDownload = e.ToDownload;
            userControl.ViewModel.DownloadTimeLeft = $"Estimated time left: {e.TimeLeft} ({e.Downloaded} of {e.ToDownload} downloaded)";
            userControl.ViewModel.DownloadTimeSpent = $"Time spent: {e.TimeSpent}";
        }

        private void BeatSaverApi_DownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            OnlineBeatmapDownloadingUserControl userControl = new OnlineBeatmapDownloadingUserControl(e.Song);
            Downloading.Add(userControl);
        }
    }
}
