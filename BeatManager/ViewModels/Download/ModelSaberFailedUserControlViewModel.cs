using ModelSaber.Entities;
using System.ComponentModel;

namespace BeatManager.ViewModels.Download
{
    public class ModelSaberFailedUserControlViewModel : INotifyPropertyChanged
    {
        private OnlineModel model;
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
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

        public ModelSaberFailedUserControlViewModel(OnlineModel model, string message)
        {
            Model = model;
            Message = message;
        }
    }
}
