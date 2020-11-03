using BeatManager.ViewModels.ModelSaberModels;
using ModelSaber.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for SaberLocalUserControl.xaml
    /// </summary>
    public partial class ModelSaberLocalUserControl : UserControl
    {
        public readonly ModelSaberLocalUserControlViewModel ViewModel;

        public ModelSaberLocalUserControl(MainWindow mainWindow, ModelType modelType)
        {
            InitializeComponent();
            ViewModel = new ModelSaberLocalUserControlViewModel(mainWindow, this, modelType);
            DataContext = ViewModel;
        }

        private void DataGridModels_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            MainWindow.ToggleLoading(false);
            ViewModel.UpdatePageButtons();
        }

        private void DataGridModels_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            LocalModel localModel = (LocalModel)e.Row.Item;
            if (localModel.Page != ViewModel.LocalModels.CurrentPage)
                e.Row.Visibility = Visibility.Collapsed;
        }

        private void ButtonBigCover_Click(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Tag.ToString();
            ViewModel.OpenBigCover(name);
        }

        private void Model_Delete(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Tag.ToString();
            ViewModel.DeleteModel(name);
        }

        private void Model_Details(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Tag.ToString();
            ViewModel.ModelDetails(name);
        }

        private void ButtonReloadData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GetModels(ViewModel.LocalModels);
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
            foreach (LocalModel model in dataGridModels.SelectedItems)
                ViewModel.SelectedModels.Add(model);
        }

        private void ContextMenuDataGridModels_Opened(object sender, RoutedEventArgs e)
        {
            int modelsToDelete = ViewModel.SelectedModels.Count();
            ViewModel.SelectedModelsToDelete = modelsToDelete;

            if (modelsToDelete == 0)
            {
                menuItemDataGridModelsDelete.Visibility = Visibility.Collapsed;
                contextMenuDataGridModels.IsOpen = false;
            }
            else
                menuItemDataGridModelsDelete.Visibility = Visibility.Visible;
        }

        private void MenuItemDataGridModelsDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteModels(ViewModel.SelectedModels.ToList());
        }
    }
}
