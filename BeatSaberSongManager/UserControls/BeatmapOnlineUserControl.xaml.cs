using BeatSaberSongManager.ViewModels;
using BeatSaverApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeatSaberSongManager.UserControls
{
    /// <summary>
    /// Interaction logic for BeatmapOnlineUserControl.xaml
    /// </summary>
    public partial class BeatmapOnlineUserControl : UserControl
    {
        public BeatMapOnlineUserControlViewModel ViewModel;

        public BeatmapOnlineUserControl()
        {
            InitializeComponent();
            ViewModel = new BeatMapOnlineUserControlViewModel();
            DataContext = ViewModel;
        }

        private void Map_Download(object sender, RoutedEventArgs e)
        {
            string sondId = ((Button)sender).Tag.ToString();
        }

        private void Map_Details(object sender, RoutedEventArgs e)
        {
            string songId = ((Button)sender).Tag.ToString();
        }

        private void RadioButtonHot_Checked(object sender, RoutedEventArgs e)
        {

            ViewModel.GetBeatSaverMaps(MapSort.Hot);
        }
    }
}
