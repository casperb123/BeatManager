using BeatManager.ViewModels.Navigation;
using ModelSaber.Entities;
using System.Windows;
using System.Windows.Controls;

namespace BeatManager.UserControls.Navigation
{
    /// <summary>
    /// Interaction logic for NavigationSabersUserControl.xaml
    /// </summary>
    public partial class NavigationModelSaberUserControl : UserControl
    {
        public readonly NavigationSabersUserControlViewModel ViewModel;

        public NavigationModelSaberUserControl(ModelType modelType)
        {
            InitializeComponent();
            ViewModel = new NavigationSabersUserControlViewModel(modelType);
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
