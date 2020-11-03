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
    /// Interaction logic for ModelSaberLocalDetailsUserControl.xaml
    /// </summary>
    public partial class ModelSaberLocalDetailsUserControl : UserControl
    {
        public readonly ModelSaberLocalDetailsUserControlViewModel ViewModel;

        public ModelSaberLocalDetailsUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new ModelSaberLocalDetailsUserControlViewModel(this, mainWindow);
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
