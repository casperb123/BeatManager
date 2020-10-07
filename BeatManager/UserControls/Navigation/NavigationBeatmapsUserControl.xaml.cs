using BeatManager.ViewModels.Navigation;
using System.Windows;
using System.Windows.Controls;

namespace BeatManager.UserControls.Navigation
{
    /// <summary>
    /// Interaction logic for NavigationBeatmapsUserControl.xaml
    /// </summary>
    public partial class NavigationBeatmapsUserControl : UserControl
    {
        public NavigationBeatmapsUserControlViewModel ViewModel;

        public NavigationBeatmapsUserControl()
        {
            InitializeComponent();
            ViewModel = new NavigationBeatmapsUserControlViewModel();
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
