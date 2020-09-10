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
        private readonly SettingsUserControlViewModel viewModel;

        public SettingsUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            viewModel = new SettingsUserControlViewModel(mainWindow);
            DataContext = viewModel;

            comboBoxTheme.ItemsSource = ThemeManager.Current.BaseColors;
            comboBoxColor.ItemsSource = ThemeManager.Current.ColorSchemes;
            viewModel.ChangeTheme(comboBoxTheme.SelectedItem.ToString(), comboBoxColor.SelectedItem.ToString());
        }

        private void ComboBoxThemeSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            viewModel.ChangeTheme(comboBoxTheme.SelectedItem.ToString(), comboBoxColor.SelectedItem.ToString());
        }

        private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            viewModel.BrowsePath();
        }

        private void ButtonDetectPath_Click(object sender, RoutedEventArgs e)
        {
            viewModel.DetectPath(Settings.CurrentSettings.BeatSaberCopy, false);
        }

        private void ToggleSwitchVersion_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;

            viewModel.DetectPath(Settings.CurrentSettings.BeatSaberCopy);
        }
    }
}
