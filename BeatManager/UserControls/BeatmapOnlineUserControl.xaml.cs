using BeatManager.ViewModels;
using BeatSaverApi.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BeatManager.UserControls
{
    /// <summary>
    /// Interaction logic for BeatmapOnlineUserControl.xaml
    /// </summary>
    public partial class BeatmapOnlineUserControl : UserControl
    {
        public BeatmapOnlineUserControlViewModel ViewModel;

        public BeatmapOnlineUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new BeatmapOnlineUserControlViewModel(mainWindow, this);
            DataContext = ViewModel;
        }

        private void Map_Download(object sender, RoutedEventArgs e)
        {
            string songKey = ((Button)sender).Tag.ToString();
            ViewModel.DownloadSong(songKey);
            ViewModel.UpdatePageButtons();
        }

        private void Map_Delete(object sender, RoutedEventArgs e)
        {
            string songKey = ((Button)sender).Tag.ToString();
            ViewModel.DeleteSong(songKey);
        }

        private void Map_Details(object sender, RoutedEventArgs e)
        {
            string songKey = ((Button)sender).Tag.ToString();
            ViewModel.BeatmapDetails(songKey);
        }

        private void RadioButtonSearch_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentMapSort = MapSort.Search;
            ViewModel.OnlineBeatmaps = null;
        }

        private void RadioButtonHot_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentMapSort = MapSort.Hot;
            ViewModel.GetBeatmaps(MapSort.Hot);
        }

        private void RadioButtonRating_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentMapSort = MapSort.Rating;
            ViewModel.GetBeatmaps(MapSort.Rating);
        }

        private void RadioButtonLatest_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentMapSort = MapSort.Latest;
            ViewModel.GetBeatmaps(MapSort.Latest);
        }

        private void RadioButtonDownloads_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentMapSort = MapSort.Downloads;
            ViewModel.GetBeatmaps(MapSort.Downloads);
        }

        private void RadioButtonPlays_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentMapSort = MapSort.Plays;
            ViewModel.GetBeatmaps(MapSort.Plays);
        }

        private void RadioButtonSearchKey_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentMapSort = MapSort.SearchKey;
            ViewModel.OnlineBeatmaps = null;
        }

        private void RadioButtonSearchHash_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentMapSort = MapSort.SearchHash;
            ViewModel.OnlineBeatmaps = null;
        }

        private void DataGridMaps_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            MainWindow.ToggleLoading(false);
            ViewModel.UpdatePageButtons();
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Search(textBoxSearch.Text);
        }

        private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ViewModel.Search(textBoxSearch.Text);
        }

        private void ButtonFirstPage_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentMapSort == MapSort.Search)
                ViewModel.FirstPage(textBoxSearch.Text);
            else
                ViewModel.FirstPage();
        }

        private void ButtonPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentMapSort == MapSort.Search)
                ViewModel.PreviousPage(textBoxSearch.Text);
            else
                ViewModel.PreviousPage();
        }

        private void ButtonNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentMapSort == MapSort.Search)
                ViewModel.NextPage(textBoxSearch.Text);
            else
                ViewModel.NextPage();
        }

        private void ButtonLastPage_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentMapSort == MapSort.Search)
                ViewModel.LastPage(textBoxSearch.Text);
            else
                ViewModel.LastPage();
        }

        private void DataGridMaps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedSongs.Clear();
            foreach (OnlineBeatmap song in dataGridMaps.SelectedItems)
                ViewModel.SelectedSongs.Add(song);

            ViewModel.SelectedSongsToDownloadCount = ViewModel.SelectedSongs.Count;
        }

        private void ContextMenuDataGridMaps_Opened(object sender, RoutedEventArgs e)
        {
            int songsToDownload = ViewModel.SelectedSongs.Where(x => !x.IsDownloaded).Count();
            int songsToDelete = ViewModel.SelectedSongs.Where(x => x.IsDownloaded).Count();

            ViewModel.SelectedSongsToDownloadCount = songsToDownload;
            ViewModel.SelectedSongsToDeleteCount = songsToDelete;

            if (songsToDownload == 0)
                menuItemDataGridMapsDownload.Visibility = Visibility.Collapsed;
            else
                menuItemDataGridMapsDownload.Visibility = Visibility.Visible;

            if (songsToDelete == 0)
                menuItemDataGridMapsDelete.Visibility = Visibility.Collapsed;
            else
                menuItemDataGridMapsDelete.Visibility = Visibility.Visible;

            if (songsToDownload == 0 && songsToDelete == 0)
                contextMenuDataGridMaps.IsOpen = false;
        }

        private void MenuItemDataGridMapsDownload_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DownloadSongs(ViewModel.SelectedSongs.Where(x => !x.IsDownloaded).ToList());
        }

        private void MenuItemDataGridMapsDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteSongs(ViewModel.SelectedSongs.Where(x => x.IsDownloaded).ToList());
        }
    }
}
