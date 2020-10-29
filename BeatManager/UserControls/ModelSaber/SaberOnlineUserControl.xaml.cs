using BeatManager.ViewModels.ModelSaberModels;
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
    public partial class SaberOnlineUserControl : UserControl
    {
        public readonly SaberOnlineUserControlViewModel ViewModel;

        public SaberOnlineUserControl(MainWindow mainWindow, ModelType modelType)
        {
            InitializeComponent();
            ViewModel = new SaberOnlineUserControlViewModel(mainWindow, this, modelType);
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
    }
}
