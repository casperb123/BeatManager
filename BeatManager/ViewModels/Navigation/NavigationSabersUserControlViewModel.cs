using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BeatManager.ViewModels.Navigation
{
    public class NavigationSabersUserControlViewModel : INotifyPropertyChanged
    {
        private bool onlinePage;
        private bool localPage;

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

        public void Local()
        {
            LocalEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Online()
        {
            OnlineEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
