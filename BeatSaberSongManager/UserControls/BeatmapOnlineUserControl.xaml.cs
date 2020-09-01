using BeatSaberSongManager.ViewModels;
using BeatSaverApi;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for BeatmapOnlineUserControl.xaml
    /// </summary>
    public partial class BeatmapOnlineUserControl : UserControl
    {
        public BeatMapOnlineUserControlViewModel ViewModel;

        public BeatmapOnlineUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new BeatMapOnlineUserControlViewModel(mainWindow);
            DataContext = ViewModel;
        }

        private void Map_Download(object sender, RoutedEventArgs e)
        {
            string sondId = ((Button)sender).Tag.ToString();
        }

        private void Map_Details(object sender, RoutedEventArgs e)
        {
            string songId = ((Button)sender).Tag.ToString();
        }

        private void RadioButtonSearch_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.BeatSaverMaps = null;
        }

        private void RadioButtonHot_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.GetBeatSaverMaps(MapSort.Hot);
        }

        private void RadioButtonRating_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.GetBeatSaverMaps(MapSort.Rating);
        }

        private void RadioButtonLatest_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.GetBeatSaverMaps(MapSort.Latest);
        }

        private void RadioButtonDownloads_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.GetBeatSaverMaps(MapSort.Downloads);
        }

        private void RadioButtonPlays_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.GetBeatSaverMaps(MapSort.Plays);
        }

        private void DataGridMaps_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ViewModel.MainWindow.rectangleLoading.Visibility = Visibility.Hidden;
            ViewModel.MainWindow.progressRingLoading.Visibility = Visibility.Hidden;
            ViewModel.MainWindow.progressRingLoading.IsActive = false;
        }

        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                await ViewModel.MainWindow.ShowMessageAsync("Beat Saver search", "The search query can't be null or empty");
                return;
            }
            if (!radioButtonSearch.IsChecked.Value)
                radioButtonSearch.IsChecked = true;

            ViewModel.GetBeatSaverMaps(textBoxSearch.Text);
        }
    }
}
