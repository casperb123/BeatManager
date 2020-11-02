using BeatManager.ViewModels.BeatSaverModels;
using BeatSaver.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BeatManager.UserControls.BeatSaver
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
            MainWindow.ToggleLoading(false);
            ViewModel.UpdatePageButtons();
        }

        private void ButtonBigCover_Click(object sender, RoutedEventArgs e)
        {
            LocalIdentifier identifier = ((Button)sender).Tag as LocalIdentifier;

            ViewModel.Beatmap = ViewModel.LocalBeatmaps.Maps.FirstOrDefault(x => x.Identifier == identifier);
            ViewModel.OpenBigCover();
        }

        private void DoubleAnimation_CloseCover(object sender, System.EventArgs e)
        {
            ViewModel.CloseBigCover();
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
            foreach (LocalBeatmap song in dataGridMaps.SelectedItems)
                ViewModel.SelectedSongs.Add(song);

            ViewModel.SelectedSongsCount = ViewModel.SelectedSongs.Count;
        }

        private void ContextMenuDataGridMaps_Opened(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedSongs.Count == 0)
            {
                menuItemDataGridMapsDelete.Visibility = Visibility.Collapsed;
                contextMenuDataGridMaps.IsOpen = false;
            }
            else
                menuItemDataGridMapsDelete.Visibility = Visibility.Visible;
        }

        private void MenuItemDataGridMapsDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteSongs(ViewModel.SelectedSongs);
        }

        private void DataGridMaps_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            LocalBeatmap localBeatmap = (LocalBeatmap)e.Row.Item;
            if (localBeatmap.Page != ViewModel.LocalBeatmaps.CurrentPage)
                e.Row.Visibility = Visibility.Collapsed;
        }

        private void ButtonReloadData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GetBeatmaps();
        }
    }
}
