using BeatManager.UserControls;
using BeatManager.UserControls.Download;
using BeatSaver.Entities;
using ModelSaber.Entities;
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

            App.ModelSaberApi.DownloadStarted += ModelSaberApi_DownloadStarted;
            App.ModelSaberApi.DownloadProgressed += ModelSaberApi_DownloadProgressed;
            App.ModelSaberApi.DownloadCompleted += ModelSaberApi_DownloadCompleted;
            App.ModelSaberApi.DownloadFailed += ModelSaberApi_DownloadFailed;
        }

        private void BeatSaverApi_DownloadFailed(object sender, BeatSaver.Events.DownloadFailedEventArgs e)
        {
            BeatmapDownloadingUserControl downloadingUserControl = GetDownloadingBeatmap(e.Beatmap);
            BeatmapCompletedUserControl completedUserControl = GetCompletedBeatmap(e.Beatmap);
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

        private void BeatSaverApi_DownloadCompleted(object sender, BeatSaver.Events.DownloadCompletedEventArgs e)
        {
            BeatmapDownloadingUserControl downloadingUserControl = GetDownloadingBeatmap(e.Beatmap);
            if (downloadingUserControl is null)
                return;

            BeatmapCompletedUserControl completedUserControl = new BeatmapCompletedUserControl(e.Beatmap, downloadingUserControl.ViewModel.ToDownload);

            userControl.stackPanelDownloading.Children.Remove(downloadingUserControl);
            DownloadingCount--;
            userControl.stackPanelCompleted.Children.Insert(0, completedUserControl);
            CompletedCount++;
        }

        private void BeatSaverApi_DownloadProgressed(object sender, BeatSaver.Events.DownloadProgressedEventArgs e)
        {
            BeatmapDownloadingUserControl downloadingUserControl = GetDownloadingBeatmap(e.Beatmap);
            if (downloadingUserControl is null)
                return;

            downloadingUserControl.ViewModel.ToDownload = e.ToDownload;
            downloadingUserControl.ViewModel.DownloadTimeLeft = $"Estimated time left: {e.TimeLeft} ({e.Downloaded} of {e.ToDownload} downloaded)";
            downloadingUserControl.ViewModel.DownloadTimeSpent = $"Time spent: {e.TimeSpent}";
            downloadingUserControl.ViewModel.ProgressPercent = e.ProgressPercent;
        }

        private void BeatSaverApi_DownloadStarted(object sender, BeatSaver.Events.DownloadStartedEventArgs e)
        {
            BeatmapDownloadingUserControl downloadingUserControl = new BeatmapDownloadingUserControl(e.Beatmap);
            userControl.stackPanelDownloading.Children.Insert(0, downloadingUserControl);
            DownloadingCount++;
        }

        private void ModelSaberApi_DownloadFailed(object sender, ModelSaber.Events.DownloadFailedEventArgs e)
        {
            ModelSaberDownloadingUserControl downloadingUserControl = GetDownloadingModelSaber(e.Model);
            ModelSaberCompletedUserControl completedUserControl = GetCompletedModelSaber(e.Model);
            ModelSaberFailedUserControl failedUserControl = new ModelSaberFailedUserControl(e.Model, e.Exception);

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

        private void ModelSaberApi_DownloadCompleted(object sender, ModelSaber.Events.DownloadCompletedEventArgs e)
        {
            ModelSaberDownloadingUserControl downloadingUserControl = GetDownloadingModelSaber(e.Model);
            if (downloadingUserControl is null)
                return;

            ModelSaberCompletedUserControl completedUserControl = new ModelSaberCompletedUserControl(e.Model, downloadingUserControl.ViewModel.ToDownload);

            userControl.stackPanelDownloading.Children.Remove(downloadingUserControl);
            DownloadingCount--;
            userControl.stackPanelCompleted.Children.Insert(0, completedUserControl);
            CompletedCount++;
        }

        private void ModelSaberApi_DownloadProgressed(object sender, ModelSaber.Events.DownloadProgressedEventArgs e)
        {
            ModelSaberDownloadingUserControl downloadingUserControl = GetDownloadingModelSaber(e.Model);
            if (downloadingUserControl is null)
                return;

            downloadingUserControl.ViewModel.ToDownload = e.ToDownload;
            downloadingUserControl.ViewModel.DownloadTimeLeft = $"Estimated time left: {e.TimeLeft} ({e.Downloaded} of {e.ToDownload} downloaded)";
            downloadingUserControl.ViewModel.DownloadTimeSpent = $"Time spent: {e.TimeSpent}";
            downloadingUserControl.ViewModel.ProgressPercent = e.ProgressPercent;
        }

        private void ModelSaberApi_DownloadStarted(object sender, ModelSaber.Events.DownloadStartedEventArgs e)
        {
            ModelSaberDownloadingUserControl downloadingUserControl = new ModelSaberDownloadingUserControl(e.Model);
            userControl.stackPanelDownloading.Children.Insert(0, downloadingUserControl);
            DownloadingCount++;
        }

        private BeatmapDownloadingUserControl GetDownloadingBeatmap(OnlineBeatmap beatmap)
        {
            BeatmapDownloadingUserControl downloadingUserControl = null;
            foreach (object obj in userControl.stackPanelDownloading.Children)
            {
                if (obj is BeatmapDownloadingUserControl control)
                {
                    if (control.ViewModel.Beatmap == beatmap)
                    {
                        downloadingUserControl = control;
                        break;
                    }
                }
            }

            return downloadingUserControl;
        }

        private BeatmapCompletedUserControl GetCompletedBeatmap(OnlineBeatmap beatmap)
        {
            BeatmapCompletedUserControl completedUserControl = null;
            foreach (object obj in userControl.stackPanelCompleted.Children)
            {
                if (obj is BeatmapCompletedUserControl control)
                {
                    if (control.ViewModel.Beatmap == beatmap)
                    {
                        completedUserControl = control;
                        break;
                    }
                }
            }

            return completedUserControl;
        }

        private ModelSaberDownloadingUserControl GetDownloadingModelSaber(OnlineModel model)
        {
            ModelSaberDownloadingUserControl downloadingUserControl = null;
            foreach (object obj in userControl.stackPanelDownloading.Children)
            {
                if (obj is ModelSaberDownloadingUserControl control)
                {
                    if (control.ViewModel.Model == model)
                    {
                        downloadingUserControl = control;
                        break;
                    }
                }
            }

            return downloadingUserControl;
        }

        private ModelSaberCompletedUserControl GetCompletedModelSaber(OnlineModel model)
        {
            ModelSaberCompletedUserControl completedUserControl = null;
            foreach (object obj in userControl.stackPanelCompleted.Children)
            {
                if (obj is ModelSaberCompletedUserControl control)
                {
                    if (control.ViewModel.Model == model)
                    {
                        completedUserControl = control;
                        break;
                    }
                }
            }

            return completedUserControl;
        }
    }
}
