using BeatSaberSongManager.ViewModels;
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
            ViewModel = new BeatmapLocalDetailsUserControlViewModel(mainWindow);
            DataContext = ViewModel;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Back();
        }
    }
}
