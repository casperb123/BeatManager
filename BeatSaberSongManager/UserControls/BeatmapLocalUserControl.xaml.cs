﻿using BeatSaberSongManager.ViewModels;
using BeatSaverApi.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BeatSaberSongManager.UserControls
{
    /// <summary>
    /// Interaction logic for BeatmapLocalUserControl.xaml
    /// </summary>
    public partial class BeatmapLocalUserControl : UserControl
    {
        public BeatmapLocalUserControlViewModel ViewModel;

        public BeatmapLocalUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new BeatmapLocalUserControlViewModel(this, mainWindow);
            DataContext = ViewModel;
        }

        private void DataGridMaps_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ViewModel.MainWindow.rectangleLoading.Visibility = Visibility.Hidden;
            ViewModel.MainWindow.progressRingLoading.Visibility = Visibility.Hidden;
            ViewModel.MainWindow.progressRingLoading.IsActive = false;
            ViewModel.UpdatePageButtons();
        }

        private void Map_Delete(object sender, RoutedEventArgs e)
        {
            LocalIdentifier identifier = ((Button)sender).Tag as LocalIdentifier;
            ViewModel.DeleteSong(identifier);
        }

        private void Map_Details(object sender, RoutedEventArgs e)
        {
            LocalIdentifier identifier = ((Button)sender).Tag as LocalIdentifier;
            ViewModel.BeatmapDetails(identifier);
        }

        private void MenuItemDataGridDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteSongs(ViewModel.SelectedSongs);
        }

        private void ButtonFirstPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FirstPage();
        }

        private void ButtonPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PreviousPage();
        }

        private void ButtonNextPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NextPage();
        }

        private void ButtonLastPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LastPage();
        }

        private void DataGridMaps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedSongs.Clear();
            ViewModel.SelectedSongsCount = dataGridMaps.SelectedItems.Count;
            foreach (LocalBeatmap song in dataGridMaps.SelectedItems)
                ViewModel.SelectedSongs.Add(song);
        }

        private void DataGridMapsContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedSongs.Count == 0)
                menuItemDataGridDelete.IsEnabled = false;
            else
                menuItemDataGridDelete.IsEnabled = true;
        }

        private void DataGridMaps_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            LocalBeatmap localBeatmap = (LocalBeatmap)e.Row.Item;
            if (localBeatmap.Page != ViewModel.LocalBeatmaps.CurrentPage)
                e.Row.Visibility = Visibility.Collapsed;
        }
    }
}
