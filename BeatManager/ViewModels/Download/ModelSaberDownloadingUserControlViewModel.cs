using ModelSaber.Entities;
using System.ComponentModel;

namespace BeatManager.ViewModels.Download
{
    public class ModelSaberDownloadingUserControlViewModel : INotifyPropertyChanged
    {
        private OnlineModel model;
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

        public OnlineModel Model
        {
            get { return model; }
            set
            {
                model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ModelSaberDownloadingUserControlViewModel(OnlineModel model)
        {
            Model = model;
        }
    }
}
