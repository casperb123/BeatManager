using BeatSaberSongManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeatSaberSongManager.UserControls
{
    /// <summary>
    /// Interaction logic for BeatmapOnlineDetailsUserControl.xaml
    /// </summary>
    public partial class BeatmapOnlineDetailsUserControl : UserControl
    {
        public readonly BeatmapOnlineDetailsUserControlViewModel ViewModel;

        public BeatmapOnlineDetailsUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new BeatmapOnlineDetailsUserControlViewModel(this, mainWindow);
            DataContext = ViewModel;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Back();
        }

        private void ButtonDownloadBeatmap_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DownloadSong();
        }

        private void ButtonDeleteBeatmap_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteSong();
        }

        private void ButtonOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFolder();
        }

        private void ButtonOpenOnBeatsaver_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenBeatSaver();
        }

        private void ButtonPreviewBeatmap_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PreviewBeatmap();
        }

        private void ButtonRefreshData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshData();
        }

        private void RadioButtonEasyDifficulty_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.Difficulty = ViewModel.Characteristic.Difficulties.Easy;
        }

        private void RadioButtonNormalDifficulty_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.Difficulty = ViewModel.Characteristic.Difficulties.Normal;
        }

        private void RadioButtonHardDifficulty_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.Difficulty = ViewModel.Characteristic.Difficulties.Hard;
        }

        private void RadioButtonExpertDifficulty_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.Difficulty = ViewModel.Characteristic.Difficulties.Expert;
        }

        private void RadioButtonExpertPlusDifficulty_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.Difficulty = ViewModel.Characteristic.Difficulties.ExpertPlus;
        }
    }
}
