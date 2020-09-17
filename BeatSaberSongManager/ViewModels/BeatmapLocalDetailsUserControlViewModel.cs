﻿using BeatSaberSongManager.UserControls;
using BeatSaverApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace BeatSaberSongManager.ViewModels
{
    public class BeatmapLocalDetailsUserControlViewModel : INotifyPropertyChanged
    {
        private readonly MainWindow mainWindow;
        private LocalBeatmap beatmap;

        public LocalBeatmap Beatmap
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

        public BeatmapLocalDetailsUserControlViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void Back()
        {
            mainWindow.transitionControl.Content = mainWindow.LocalUserControl;
        }
    }
}
