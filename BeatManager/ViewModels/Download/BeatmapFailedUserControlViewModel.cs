using BeatSaver.Entities;
using System;
using System.ComponentModel;

namespace BeatManager.ViewModels.Download
{
    public class BeatmapFailedUserControlViewModel : INotifyPropertyChanged
    {
        private OnlineBeatmap beatmap;
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
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

        public BeatmapFailedUserControlViewModel(OnlineBeatmap beatmap, string message)
        {
            Beatmap = beatmap;
            Message = message;
        }
    }
}
