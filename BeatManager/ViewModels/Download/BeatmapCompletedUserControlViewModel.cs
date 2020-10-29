using BeatSaver.Entities;
using System.ComponentModel;

namespace BeatManager.ViewModels.Download
{
    public class BeatmapCompletedUserControlViewModel : INotifyPropertyChanged
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

        public BeatmapCompletedUserControlViewModel(OnlineBeatmap beatmap, string downloaded)
        {
            Beatmap = beatmap;
            Downloaded = downloaded;
        }
    }
}
