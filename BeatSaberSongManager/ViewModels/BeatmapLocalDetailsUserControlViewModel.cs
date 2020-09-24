using BeatSaberSongManager.UserControls;
using BeatSaverApi.Entities;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private LocalBeatmapDetails beatmapDetails;
        private LocalBeatmapDetail beatmapDetail;
        private List<string> errors;

        public List<string> Errors
        {
            get { return errors; }
            set
            {
                errors = value;
                OnPropertyChanged(nameof(Errors));
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

        public LocalBeatmapDetails BeatmapDetails
        {
            get { return beatmapDetails; }
            set
            {
                beatmapDetails = value;
                OnPropertyChanged(nameof(BeatmapDetails));
                CreateDifficulties();
            }
        }

        public LocalBeatmap Beatmap
        {
            get { return beatmap; }
            set
            {
                beatmap = value;
                OnPropertyChanged(nameof(Beatmap));
                CreateDifficultySets();
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
            mainWindow.ViewModel.ShowLocalDetails = false;
            mainWindow.transitionControl.Content = mainWindow.ViewModel.LocalUserControl;
        }

        private void CreateDifficultySets()
        {
            userControl.stackPanelSets.Children.Clear();

            foreach (LocalBeatmapDetails beatmapDetails in Beatmap.Details)
            {
                RadioButton radioButton = XamlReader.Parse(XamlWriter.Save(userControl.radioButtonDifficultyTemplate)) as RadioButton;
                radioButton.Style = userControl.Resources[$"ToggleButtonDifficultySet"] as Style;

                radioButton.Content = beatmapDetails.CharacteristicName;
                radioButton.Visibility = Visibility.Visible;
                radioButton.Checked += (s, e) => BeatmapDetails = beatmapDetails;

                userControl.stackPanelSets.Children.Add(radioButton);
            }

            ((RadioButton)userControl.stackPanelSets.Children[0]).IsChecked = true;
        }

        private void CreateDifficulties()
        {
            userControl.stackPanelDifficulties.Children.Clear();

            foreach (LocalBeatmapDetail beatmapDetail in BeatmapDetails.BeatmapDetails)
            {
                RadioButton radioButton = XamlReader.Parse(XamlWriter.Save(userControl.radioButtonDifficultyTemplate)) as RadioButton;
                radioButton.Style = userControl.Resources[$"ToggleButtonDifficulty{beatmapDetail.DifficultyBeatmap.Difficulty}"] as Style;

                if (beatmapDetail.DifficultyBeatmap.Difficulty == "ExpertPlus")
                    radioButton.Content = "Expert+";
                else
                    radioButton.Content = beatmapDetail.DifficultyBeatmap.Difficulty;

                radioButton.Visibility = Visibility.Visible;
                radioButton.Checked += (s, e) => BeatmapDetail = beatmapDetail;

                userControl.stackPanelDifficulties.Children.Add(radioButton);
            }

            ((RadioButton)userControl.stackPanelDifficulties.Children[0]).IsChecked = true;
        }

        public void DeleteSong()
        {
            mainWindow.ViewModel.LocalUserControl.ViewModel.DeleteSong(Beatmap.Identifier);
            mainWindow.transitionControl.Content = mainWindow.ViewModel.LocalUserControl;
        }

        public void RefreshData()
        {
            mainWindow.ViewModel.LocalUserControl.ViewModel.BeatmapDetails(Beatmap.Identifier, false);
        }

        public void OpenFolder()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = beatmap.FolderPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void PreviewBeatmap()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://skystudioapps.com/bs-viewer/?id={beatmap.OnlineBeatmap.Key}",
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void OpenBeatSaver()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://beatsaver.com/beatmap/{beatmap.OnlineBeatmap.Key}",
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public async void ShowErrors()
        {
            string errorsText = string.Join("\n", Errors);
            await mainWindow.ShowMessageAsync("Beatmap Invalid", $"The current beatmap has the following errors:\n" +
                                                                 $"{errorsText}");
        }
    }
}
