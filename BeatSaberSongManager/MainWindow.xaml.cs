using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using BeatSaberSongManager.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
        private readonly MainWindowViewModel viewModel;
        private readonly BeatmapLocalUserControl localUserControl;
        private readonly BeatmapOnlineUserControl onlineUserControl;
        private readonly SettingsUserControl settingsUserControl;
        private bool loadLocalBeatmaps = true;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            localUserControl = new BeatmapLocalUserControl(this);
            onlineUserControl = new BeatmapOnlineUserControl(this);
            settingsUserControl = new SettingsUserControl(this);
        }

        private void RadioButtonLocal_Checked(object sender, RoutedEventArgs e)
        {
            userControlMain.Content = localUserControl;
            if (loadLocalBeatmaps)
                localUserControl.ViewModel.GetBeatmaps();
        }

        private async void RadioButtonOnline_Checked(object sender, RoutedEventArgs e)
        {
            if (!viewModel.IsConnectedToInternet())
            {
                await this.ShowMessageAsync("No internet connection", "You don't have access to the internet");
                if (userControlMain.Content == localUserControl)
                {
                    loadLocalBeatmaps = false;
                    radioButtonLocal.IsChecked = true;
                    loadLocalBeatmaps = true;
                }
                else
                    radioButtonSettings.IsChecked = true;

                return;
            }

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
