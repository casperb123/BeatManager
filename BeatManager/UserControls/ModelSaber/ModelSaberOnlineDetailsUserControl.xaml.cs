using BeatManager.ViewModels.ModelSaberModels;
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
    /// Interaction logic for ModelSaberOnlineDetailsUserControl.xaml
    /// </summary>
    public partial class ModelSaberOnlineDetailsUserControl : UserControl
    {
        public readonly ModelSaberOnlineDetailsUserControlViewModel ViewModel;

        public ModelSaberOnlineDetailsUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new ModelSaberOnlineDetailsUserControlViewModel(this, mainWindow);
            DataContext = ViewModel;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Back();
        }

        private void ButtonBigCover_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenBigCover();
        }

        private async void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.DownloadModel();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteModel();
        }

        private void ButtonRefreshData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshData();
        }

        private void ButtonOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFolder();
        }

        private void ButtonOpenModelSaber_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenModelSaber();
        }
    }
}
