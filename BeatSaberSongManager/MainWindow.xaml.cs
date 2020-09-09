using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using BeatSaberSongManager.ViewModels;
using BeatSaverApi.Entities;
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

            if (viewModel.CanConnectToBeatSaver())
            {
                if (localUserControl.ViewModel.SongDeleted)
                {
                    if (onlineUserControl.ViewModel.OnlineBeatmaps != null)
                    {
                        MapSort mapSort = onlineUserControl.ViewModel.CurrentMapSort;
                        if (mapSort == MapSort.Search)
                            onlineUserControl.ViewModel.GetBeatmaps(onlineUserControl.textBoxSearch.Text, onlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                        else
                            onlineUserControl.ViewModel.GetBeatmaps(mapSort, onlineUserControl.ViewModel.OnlineBeatmaps.CurrentPage);
                    }

                    localUserControl.ViewModel.SongDeleted = false;
                }
            }
            else
            {
                await this.ShowMessageAsync("Can't connect to BeatSaver", "Either you don't have any internet connection or BeatSaver is currently offline");
                if (userControlMain.Content == localUserControl)
                {
                    loadLocalBeatmaps = false;
                    radioButtonLocal.IsChecked = true;
                    loadLocalBeatmaps = true;
                }
                else if (userControlMain.Content == settingsUserControl)
                    radioButtonSettings.IsChecked = true;
                else
                    radioButtonOnline.IsChecked = false;

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
