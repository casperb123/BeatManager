using BeatManager.UserControls;
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
        private readonly DownloadsUserControl userControl;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public DownloadsUserControlViewModel(DownloadsUserControl userControl)
        {
            this.userControl = userControl;

            App.BeatSaverApi.DownloadStarted += BeatSaverApi_DownloadStarted;
            App.BeatSaverApi.DownloadProgressed += BeatSaverApi_DownloadProgressed;
            App.BeatSaverApi.DownloadCompleted += BeatSaverApi_DownloadCompleted;
        }

        private void BeatSaverApi_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            OnlineBeatmapDownloadingUserControl downloadingUserControl = null;
            foreach (OnlineBeatmapDownloadingUserControl control in userControl.stackPanelDownloading.Children)
            {
                if (control.ViewModel.Beatmap == e.Song)
                {
                    downloadingUserControl = control;
                    break;
                }
            }

            if (downloadingUserControl is null)
                return;

            OnlineBeatmapCompletedUserControl completedUserControl = new OnlineBeatmapCompletedUserControl(e.Song, downloadingUserControl.ViewModel.ToDownload);
            completedUserControl.ViewModel.Downloaded = downloadingUserControl.ViewModel.ToDownload;

            userControl.stackPanelDownloading.Children.Remove(downloadingUserControl);
            DownloadingCount--;
            userControl.stackPanelCompleted.Children.Add(completedUserControl);
            CompletedCount++;
        }

        private void BeatSaverApi_DownloadProgressed(object sender, DownloadProgressedEventArgs e)
        {
            OnlineBeatmapDownloadingUserControl downloadingUserControl = null;
            foreach (OnlineBeatmapDownloadingUserControl control in userControl.stackPanelDownloading.Children)
            {
                if (control.ViewModel.Beatmap == e.Beatmap)
                {
                    downloadingUserControl = control;
                    break;
                }
            }

            if (downloadingUserControl is null)
                return;

            downloadingUserControl.ViewModel.ToDownload = e.ToDownload;
            downloadingUserControl.ViewModel.DownloadTimeLeft = $"Estimated time left: {e.TimeLeft} ({e.Downloaded} of {e.ToDownload} downloaded)";
            downloadingUserControl.ViewModel.DownloadTimeSpent = $"Time spent: {e.TimeSpent}";
        }

        private void BeatSaverApi_DownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            OnlineBeatmapDownloadingUserControl downloadingUserControl = new OnlineBeatmapDownloadingUserControl(e.Song);
            userControl.stackPanelDownloading.Children.Add(downloadingUserControl);
            DownloadingCount++;
        }
    }
}
