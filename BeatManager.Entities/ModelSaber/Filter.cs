using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BeatManager.Entities.ModelSaber
{
    public class Filter : INotifyPropertyChanged
    {
        private FilterType type;
        private string text;

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public FilterType Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public Filter(FilterType type, string text)
        {
            Type = type;
            Text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
