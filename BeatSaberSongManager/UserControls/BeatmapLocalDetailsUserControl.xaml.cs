using BeatSaberSongManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BeatSaberSongManager.UserControls
{
    /// <summary>
    /// Interaction logic for BeatmapLocalDetailsUserControl.xaml
    /// </summary>
    public partial class BeatmapLocalDetailsUserControl : UserControl
    {
        public BeatmapLocalDetailsUserControlViewModel ViewModel;

        public BeatmapLocalDetailsUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new BeatmapLocalDetailsUserControlViewModel(this, mainWindow);
            DataContext = ViewModel;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Back();
        }
    }
}
