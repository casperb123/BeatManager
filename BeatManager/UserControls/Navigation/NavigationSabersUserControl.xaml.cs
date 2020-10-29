using BeatManager.ViewModels.Navigation;
using System.Windows;
using System.Windows.Controls;

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
