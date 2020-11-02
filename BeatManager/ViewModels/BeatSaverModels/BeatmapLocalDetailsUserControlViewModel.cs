﻿using BeatManager.UserControls.BeatSaver;
using BeatSaver.Entities;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BeatManager.ViewModels.BeatSaverModels
{
    public class BeatmapLocalDetailsUserControlViewModel : INotifyPropertyChanged
    {
        private readonly BeatmapLocalDetailsUserControl userControl;
        private readonly MainWindow mainWindow;
        private LocalBeatmap beatmap;
        private LocalBeatmapDetails beatmapDetails;
        private LocalBeatmapDetail beatmapDetail;

        public LocalBeatmapDetail BeatmapDetail
        {
            get { return beatmapDetail; }
            set
            {
                beatmapDetail = value;
                OnPropertyChanged(nameof(BeatmapDetail));

                if (value.DifficultyBeatmap.CustomData is null || value.DifficultyBeatmap.CustomData.RequiredMods.Count == 0)
                {
                    userControl.dataGridDifficultyRequirements.Visibility = Visibility.Collapsed;
                    userControl.labelRequirements.Visibility = Visibility.Visible;
                }
                else
                {
                    userControl.labelRequirements.Visibility = Visibility.Collapsed;
                    userControl.dataGridDifficultyRequirements.Visibility = Visibility.Visible;
                }
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
            mainWindow.ViewModel.ShowLocalBeatmapDetails = false;
            mainWindow.userControlMain.Content = mainWindow.ViewModel.BeatmapLocalUserControl;
            CloseBigCover();
        }

        private void CreateDifficultySets()
        {
            userControl.stackPanelSets.Children.Clear();

            foreach (LocalBeatmapDetails beatmapDetails in Beatmap.Details)
            {
                RadioButton radioButton = XamlReader.Parse(XamlWriter.Save(userControl.radioButtonDifficultyTemplate)) as RadioButton;
                radioButton.Style = App.Instance.Resources[$"ToggleButtonDifficultySet"] as Style;

                radioButton.Content = beatmapDetails.CharacteristicName;
                radioButton.ToolTip = beatmapDetails.CharacteristicName;
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
                radioButton.Style = App.Instance.Resources[$"ToggleButtonDifficulty{beatmapDetail.DifficultyBeatmap.Difficulty}"] as Style;
                string difficulty = beatmapDetail.DifficultyBeatmap.Difficulty;

                if (difficulty == "ExpertPlus")
                    difficulty = "Expert+";

                if (string.IsNullOrWhiteSpace(beatmapDetail.DifficultyBeatmap.CustomData.DifficultyLabel))
                    radioButton.Content = difficulty;
                else
                    radioButton.Content = beatmapDetail.DifficultyBeatmap.CustomData.DifficultyLabel;

                radioButton.ToolTip = difficulty;
                radioButton.Visibility = Visibility.Visible;
                radioButton.Checked += (s, e) => BeatmapDetail = beatmapDetail;

                userControl.stackPanelDifficulties.Children.Add(radioButton);
            }

            ((RadioButton)userControl.stackPanelDifficulties.Children[0]).IsChecked = true;
        }

        public void DeleteSong()
        {
            mainWindow.ViewModel.BeatmapLocalUserControl.ViewModel.DeleteSong(Beatmap.Identifier);
            mainWindow.userControlMain.Content = mainWindow.ViewModel.BeatmapLocalUserControl;
        }

        public void RefreshData()
        {
            mainWindow.ViewModel.BeatmapLocalUserControl.ViewModel.BeatmapDetails(Beatmap.Identifier, false);
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
            string errorsText = string.Join("\n", Beatmap.Errors.Select(x => $"- {x}"));
            await mainWindow.ShowMessageAsync("Beatmap Invalid", $"The current beatmap has the following errors:\n" +
                                                                 $"{errorsText}");
        }

        public void OpenBigCover()
        {
            userControl.gridCoverImage.Visibility = Visibility.Visible;
        }

        public void CloseBigCover()
        {
            userControl.gridCoverImage.Visibility = Visibility.Hidden;
            userControl.gridCoverImage.Opacity = 0;
        }
    }
}