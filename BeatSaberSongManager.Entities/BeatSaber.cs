using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BeatSaberSongManager.Entities
{
    public class BeatSaber : INotifyPropertyChanged
    {
        private string path;
        private bool isCopy;

        public bool IsCopy
        {
            get { return isCopy; }
            set
            {
                isCopy = value;
                OnPropertyChanged(nameof(IsCopy));
            }
        }

        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        public BeatSaber(string path, bool isOriginal)
        {
            Path = path;
            IsCopy = isOriginal;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
