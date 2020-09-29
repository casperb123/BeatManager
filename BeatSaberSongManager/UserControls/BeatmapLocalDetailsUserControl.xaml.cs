using BeatSaberSongManager.Entities;
using BeatSaberSongManager.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeatSaberSongManager.UserControls
{
    /// <summary>
    /// Interaction logic for BeatmapLocalDetailsUserControl.xaml
    /// </summary>
    public partial class BeatmapLocalDetailsUserControl : UserControl
    {
        public BeatmapLocalDetailsUserControlViewModel ViewModel;

        public BeatmapLocalDetailsUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new BeatmapLocalDetailsUserControlViewModel(this, mainWindow);
            DataContext = ViewModel;

            dataGridDifficultyRequirements.SelectionChanged += (s, e) => dataGridDifficultyRequirements.UnselectAll();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Back();
        }

        private void ButtonDeleteBeatmap_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteSong();
        }

        private void ButtonRefreshData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshData();
        }

        private void ButtonOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFolder();
        }

        private void ButtonPreviewBeatmap_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PreviewBeatmap();
        }

        private void ButtonOpenOnBeatsaver_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenBeatSaver();
        }

        private void ButtonInvalid_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowErrors();
        }

        private void DataGridDifficultyRequirements_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            string modName = e.Row.Item.ToString().Replace(" ", "");

            if (App.SupportedMods.Contains(modName))
                e.Row.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0));
            else
                e.Row.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }
    }
}
