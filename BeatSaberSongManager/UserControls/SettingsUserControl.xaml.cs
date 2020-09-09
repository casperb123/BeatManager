using BeatSaberSongManager.ViewModels;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

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
            IEnumerable<PrimaryColor> primaryColors = Enum.GetValues(typeof(PrimaryColor)).Cast<PrimaryColor>();
            string[] themes = new string[]
            {
                "Light",
                "Dark"
            };

            comboBoxTheme.ItemsSource = themes;
            comboBoxColor.ItemsSource = primaryColors;

            viewModel.ChangeTheme(comboBoxTheme.SelectedItem.ToString(), (PrimaryColor)comboBoxColor.SelectedItem);
        }

        private void ComboBoxThemeSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            viewModel.ChangeTheme(comboBoxTheme.SelectedItem.ToString(), (PrimaryColor)comboBoxColor.SelectedItem);
        }

        private void ButtonSongsPathBrowse_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SetSongsPath(false, true);
        }
    }
}
