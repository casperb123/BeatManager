using ModelSaber.Entities;
using System;
using System.ComponentModel;

namespace BeatManager.ViewModels.Navigation
{
    public class NavigationSabersUserControlViewModel : INotifyPropertyChanged
    {
        private bool onlinePage;
        private bool localPage;

        public ModelType ModelType { get; set; }

        public event EventHandler LocalEvent;
        public event EventHandler OnlineEvent;

        public bool LocalPage
        {
            get { return localPage; }
            set
            {
                localPage = value;
                OnPropertyChanged(nameof(LocalPage));
            }
        }
        public bool OnlinePage
        {
            get { return onlinePage; }
            set
            {
                onlinePage = value;
                OnPropertyChanged(nameof(OnlinePage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public NavigationSabersUserControlViewModel(ModelType modelType)
        {
            ModelType = modelType;
        }

        public void Local()
        {
            LocalPage = true;
            OnlinePage = false;
            LocalEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Online()
        {
            OnlinePage = true;
            LocalPage = false;
            OnlineEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
