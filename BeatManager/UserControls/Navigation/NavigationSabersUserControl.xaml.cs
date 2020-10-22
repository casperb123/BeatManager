using BeatManager.ViewModels.Navigation;
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

namespace BeatManager.UserControls.Navigation
{
    /// <summary>
    /// Interaction logic for NavigationSabersUserControl.xaml
    /// </summary>
    public partial class NavigationSabersUserControl : UserControl
    {
        public readonly NavigationSabersUserControlViewModel ViewModel;

        public NavigationSabersUserControl()
        {
            InitializeComponent();
            ViewModel = new NavigationSabersUserControlViewModel();
            DataContext = ViewModel;
        }

        private void RadioButtonLocal_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Local();
        }

        private void RadioButtonOnline_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Online();
        }
    }
}
