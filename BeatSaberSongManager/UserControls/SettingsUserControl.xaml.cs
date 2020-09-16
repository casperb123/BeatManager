using BeatSaberSongManager.Entities;
using BeatSaberSongManager.ViewModels;
using ControlzEx.Theming;
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
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsUserControl : UserControl
    {
        public readonly SettingsUserControlViewModel ViewModel;

        public SettingsUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new SettingsUserControlViewModel(mainWindow);
            DataContext = ViewModel;

            comboBoxTheme.ItemsSource = ThemeManager.Current.BaseColors;
            comboBoxColor.ItemsSource = ThemeManager.Current.ColorSchemes;
            ViewModel.ChangeTheme(comboBoxTheme.SelectedItem.ToString(), comboBoxColor.SelectedItem.ToString());
        }

        private void ComboBoxThemeSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            ViewModel.ChangeTheme(comboBoxTheme.SelectedItem.ToString(), comboBoxColor.SelectedItem.ToString());
        }

        private void ToggleSwitchVersion_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;

            ViewModel.DetectPath(Settings.CurrentSettings.BeatSaberCopy);
        }

        private void ButtonDetectPath_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DetectPath(Settings.CurrentSettings.BeatSaberCopy);
        }

        private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.BrowsePath();
        }
    }
}
