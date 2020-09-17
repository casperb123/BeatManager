using BeatSaberSongManager.UserControls;
using BeatSaverApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BeatSaberSongManager.ViewModels
{
    public class BeatmapLocalDetailsUserControlViewModel : INotifyPropertyChanged
    {
        private readonly BeatmapLocalDetailsUserControl userControl;
        private readonly MainWindow mainWindow;
        private LocalBeatmap beatmap;
        private LocalBeatmapDetail beatmapDetail;
        private DifficultyBeatmap difficulty;

        public DifficultyBeatmap Difficulty
        {
            get { return difficulty; }
            set
            {
                difficulty = value;
                OnPropertyChanged(nameof(Difficulty));
            }
        }

        public LocalBeatmapDetail BeatmapDetail
        {
            get { return beatmapDetail; }
            set
            {
                beatmapDetail = value;
                OnPropertyChanged(nameof(BeatmapDetail));
            }
        }

        public LocalBeatmap Beatmap
        {
            get { return beatmap; }
            set
            {
                beatmap = value;
                OnPropertyChanged(nameof(Beatmap));
                CreateDifficulties();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public BeatmapLocalDetailsUserControlViewModel(BeatmapLocalDetailsUserControl userControl, MainWindow mainWindow)
        {
            this.userControl = userControl;
            this.mainWindow = mainWindow;
        }

        public void Back()
        {
            mainWindow.transitionControl.Content = mainWindow.LocalUserControl;
        }

        private void CreateDifficulties()
        {
            userControl.stackPanelSets.Children.Clear();

            foreach (DifficultyBeatmapSet difficultyBeatmapSet in Beatmap.DifficultyBeatmapSets)
            {
                RadioButton radioButton = XamlReader.Parse(XamlWriter.Save(userControl.radioButtonDifficultySet)) as RadioButton;

                radioButton.Content = difficultyBeatmapSet.BeatmapCharacteristicName;
                radioButton.Visibility = Visibility.Visible;
                userControl.stackPanelSets.Children.Add(radioButton);
            }
        }
    }
}
