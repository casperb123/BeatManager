using ModelSaber.Entities;
using System.ComponentModel;

namespace BeatManager.ViewModels.Download
{
    public class ModelSaberCompletedUserControlViewModel : INotifyPropertyChanged
    {
        private OnlineModel model;
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

        public ModelSaberCompletedUserControlViewModel(OnlineModel model)
        {
            Model = model;
        }
    }
}
