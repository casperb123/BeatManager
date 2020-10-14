using BeatManager.UserControls;
using BeatManager.UserControls.Download;
using BeatSaver.Entities;
using BeatSaver.Events;
using System.ComponentModel;

namespace BeatManager.ViewModels
{
    public class DownloadsUserControlViewModel : INotifyPropertyChanged
    {
        private readonly DownloadsUserControl userControl;
        private int downloadingCount;
        private int completedCount;
        private int failedCount;

        public int FailedCount
        {
            get { return failedCount; }
            set
            {
                failedCount = value;
                OnPropertyChanged(nameof(FailedCount));
            }
        }

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
            App.BeatSaverApi.DownloadFailed += BeatSaverApi_DownloadFailed;
        }

        private void BeatSaverApi_DownloadFailed(object sender, DownloadFailedEventArgs e)
        {
            BeatmapDownloadingUserControl downloadingUserControl = GetDownloading(e.Beatmap);
            BeatmapCompletedUserControl completedUserControl = GetCompleted(e.Beatmap);
            BeatmapFailedUserControl failedUserControl = new BeatmapFailedUserControl(e.Beatmap, e.Exception);

            userControl.stackPanelFailed.Children.Insert(0, failedUserControl);
            FailedCount++;

            if (downloadingUserControl != null)
            {
                userControl.stackPanelDownloading.Children.Remove(downloadingUserControl);
                DownloadingCount--;
            }
            else if (completedUserControl != null)
            {
                userControl.stackPanelCompleted.Children.Remove(completedUserControl);
                CompletedCount--;
            }
        }

        private void BeatSaverApi_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            BeatmapDownloadingUserControl downloadingUserControl = GetDownloading(e.Beatmap);
            if (downloadingUserControl is null)
                return;

            BeatmapCompletedUserControl completedUserControl = new BeatmapCompletedUserControl(e.Beatmap, downloadingUserControl.ViewModel.ToDownload);
            completedUserControl.ViewModel.Downloaded = downloadingUserControl.ViewModel.ToDownload;

            userControl.stackPanelDownloading.Children.Remove(downloadingUserControl);
            DownloadingCount--;
            userControl.stackPanelCompleted.Children.Insert(0, completedUserControl);
            CompletedCount++;
        }

        private void BeatSaverApi_DownloadProgressed(object sender, DownloadProgressedEventArgs e)
        {
            BeatmapDownloadingUserControl downloadingUserControl = GetDownloading(e.Beatmap);
            if (downloadingUserControl is null)
                return;

            downloadingUserControl.ViewModel.ToDownload = e.ToDownload;
            downloadingUserControl.ViewModel.DownloadTimeLeft = $"Estimated time left: {e.TimeLeft} ({e.Downloaded} of {e.ToDownload} downloaded)";
            downloadingUserControl.ViewModel.DownloadTimeSpent = $"Time spent: {e.TimeSpent}";
            downloadingUserControl.ViewModel.ProgressPercent = e.ProgressPercent;
        }

        private void BeatSaverApi_DownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            BeatmapDownloadingUserControl downloadingUserControl = new BeatmapDownloadingUserControl(e.Beatmap);
            userControl.stackPanelDownloading.Children.Insert(0, downloadingUserControl);
            DownloadingCount++;
        }

        private BeatmapDownloadingUserControl GetDownloading(OnlineBeatmap beatmap)
        {
            BeatmapDownloadingUserControl downloadingUserControl = null;
            foreach (BeatmapDownloadingUserControl control in userControl.stackPanelDownloading.Children)
            {
                if (control.ViewModel.Beatmap == beatmap)
                {
                    downloadingUserControl = control;
                    break;
                }
            }

            return downloadingUserControl;
        }

        private BeatmapCompletedUserControl GetCompleted(OnlineBeatmap beatmap)
        {
            BeatmapCompletedUserControl completedUserControl = null;
            foreach (BeatmapCompletedUserControl control in userControl.stackPanelCompleted.Children)
            {
                if (control.ViewModel.Beatmap == beatmap)
                {
                    completedUserControl = control;
                    break;
                }
            }

            return completedUserControl;
        }
    }
}
