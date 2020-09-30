using System.ComponentModel;

namespace BeatManager.ViewModels.Navigation
{
    public class NavigationBeatmapsUserControlViewModel : INotifyPropertyChanged
    {
        private bool onlinePage;
        private bool localPage;

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
    }
}
