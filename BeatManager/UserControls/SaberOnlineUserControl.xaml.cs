using BeatManager.ViewModels;
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

namespace BeatManager.UserControls
{
    /// <summary>
    /// Interaction logic for ModelOnlineUserControl.xaml
    /// </summary>
    public partial class SaberOnlineUserControl : UserControl
    {
        public readonly SaberOnlineUserControlViewModel ViewModel;

        public SaberOnlineUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new SaberOnlineUserControlViewModel(mainWindow, this);
            DataContext = ViewModel;
        }

        private void DataGridModels_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            MainWindow.ToggleLoading(false);
            ViewModel.UpdatePageButtons();
        }

        private void DataGridModels_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            OnlineModel onlineModel = (OnlineModel)e.Row.Item;
            if (onlineModel.Page != ViewModel.OnlineModels.CurrentPage)
                e.Row.Visibility = Visibility.Collapsed;
        }

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonSortDirection_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Saber_Download(object sender, RoutedEventArgs e)
        {

        }

        private void Saber_Delete(object sender, RoutedEventArgs e)
        {

        }

        private void Saber_Details(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonReloadData_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonFirstPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonPreviousPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonNextPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonLastPage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
