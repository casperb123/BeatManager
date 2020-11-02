using BeatManager.ViewModels.ModelSaberModels;
using Ionic.Zip;
using ModelSaber.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for ModelSaberOnlineUserControl.xaml
    /// </summary>
    public partial class ModelSaberOnlineUserControl : UserControl
    {
        public readonly ModelSaberOnlineUserControlViewModel ViewModel;

        public ModelSaberOnlineUserControl(MainWindow mainWindow, ModelType modelType)
        {
            InitializeComponent();
            ViewModel = new ModelSaberOnlineUserControlViewModel(mainWindow, this, modelType);
            DataContext = ViewModel;
        }

        private void DataGridModels_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            MainWindow.ToggleLoading(false);
            ViewModel.UpdatePageButtons();
        }

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ViewModel.IsLoaded)
                return;

            ViewModel.GetSabers();
        }

        private void ButtonSortDirection_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsLoaded)
                return;

            ViewModel.ChangeSortDirection();
        }

        private void ButtonAddFilter_Click(object sender, RoutedEventArgs e)
        {
            string filterText = textBoxFilterSearch.Text;

            if (!string.IsNullOrWhiteSpace(filterText) &&
                !ViewModel.Filters.Any(x => x.Type == ViewModel.CurrentFilterType && x.Text == filterText))
            {
                ViewModel.AddFilter(new Filter(ViewModel.CurrentFilterType, filterText));
            }
        }

        private void TextBoxFilterSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            string filterText = textBoxFilterSearch.Text;
            if (!string.IsNullOrWhiteSpace(filterText) &&
                !ViewModel.Filters.Any(x => x.Type == ViewModel.CurrentFilterType && x.Text == filterText))
            {
                ViewModel.AddFilter(new Filter(ViewModel.CurrentFilterType, filterText));
            }
        }

        private async void Saber_Download(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(((Button)sender).Tag.ToString());
            await ViewModel.DownloadModel(id);
        }

        private void Saber_Delete(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(((Button)sender).Tag.ToString());
            ViewModel.DeleteModel(id);
        }

        private void Saber_Details(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonReloadData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GetSabers(ViewModel.OnlineModels);
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

        private void DataGridModels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedModels.Clear();
            foreach (OnlineModel model in dataGridModels.SelectedItems)
                ViewModel.SelectedModels.Add(model);
        }

        private void ContextMenuDataGridModels_Opened(object sender, RoutedEventArgs e)
        {
            int modelsToDownload = ViewModel.SelectedModels.Where(x => !x.IsDownloaded).Count();
            int modelsToDelete = ViewModel.SelectedModels.Where(x => x.IsDownloaded).Count();

            ViewModel.SelectedModelsToDownload = modelsToDownload;
            ViewModel.SelectedModelsToDelete = modelsToDelete;

            if (modelsToDownload == 0)
                menuItemDataGridModelsDownload.Visibility = Visibility.Collapsed;
            else
                menuItemDataGridModelsDownload.Visibility = Visibility.Visible;

            if (modelsToDelete == 0)
                menuItemDataGridModelsDelete.Visibility = Visibility.Collapsed;
            else
                menuItemDataGridModelsDelete.Visibility = Visibility.Visible;

            if (modelsToDownload == 0 && modelsToDelete == 0)
                contextMenuDataGridModels.IsOpen = false;
        }

        private async void MenuItemDataGridModelsDownload_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.DownloadModels(ViewModel.SelectedModels.Where(x => !x.IsDownloaded).ToList());
        }

        private void MenuItemDataGridModelsDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteModels(ViewModel.SelectedModels.Where(x => x.IsDownloaded).ToList());
        }
    }
}
