using BeatManager.ViewModels.ModelSaberModels;
using ModelSaber.Entities;
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

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for SaberLocalUserControl.xaml
    /// </summary>
    public partial class SaberLocalUserControl : UserControl
    {
        public readonly SaberLocalUserControlViewModel ViewModel;

        public SaberLocalUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new SaberLocalUserControlViewModel(mainWindow, this, ModelType.Saber);
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

        private void Saber_Delete(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Tag.ToString();
        }

        private void Saber_Details(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Tag.ToString();
        }
    }
}
