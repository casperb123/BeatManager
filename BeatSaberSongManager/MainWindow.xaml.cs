using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace BeatSaberSongManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly BeatmapLocalUserControl localUserControl;
        private readonly BeatmapOnlineUserControl onlineUserControl;
        private readonly SettingsUserControl settingsUserControl;

        public MainWindow()
        {
            InitializeComponent();
            localUserControl = new BeatmapLocalUserControl();
            onlineUserControl = new BeatmapOnlineUserControl(this);
            settingsUserControl = new SettingsUserControl(this);

            radioButtonLocal.IsChecked = true;
        }

        private void RadioButtonLocal_Checked(object sender, RoutedEventArgs e)
        {
            userControlMain.Content = localUserControl;
        }

        private void RadioButtonOnline_Checked(object sender, RoutedEventArgs e)
        {
            userControlMain.Content = onlineUserControl;
        }

        private void RadioButtonSettings_Checked(object sender, RoutedEventArgs e)
        {
            userControlMain.Content = settingsUserControl;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.CurrentSettings.Save();
        }
    }
}
