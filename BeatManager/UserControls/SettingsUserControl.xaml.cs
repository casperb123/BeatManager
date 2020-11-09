using BeatManager.Entities;
using BeatManager.ViewModels;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BeatManager.UserControls
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

        private async void ToggleSwitchVersion_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;

            if (ViewModel.ChangePath)
            {
                MainWindow.ToggleLoading(true, "Locating Beat Saber folder");
                await Task.Run(async () => await ViewModel.GetBeatSaberPath(Settings.CurrentSettings.BeatSaberCopy, true));
                MainWindow.ToggleLoading(false);
            }
            else
                ViewModel.ChangePath = true;
        }

        private async void ButtonDetectPath_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ToggleLoading(true, "Locating Beat Saber folder");
            await Task.Run(async () => await ViewModel.GetBeatSaberPath(Settings.CurrentSettings.BeatSaberCopy, true));
            MainWindow.ToggleLoading(false);
        }

        private async void ToggleSwitchBeatSaverOneClick_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded || !ViewModel.IsRunningAsAdmin)
                return;

            await ViewModel.ToggleOneClick(OneClickType.BeatSaver, Settings.CurrentSettings.BeatSaver.OneClickInstaller);
        }

        private async void ToggleSwitchModelSaberOneClick_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded || !ViewModel.IsRunningAsAdmin)
                return;

            await ViewModel.ToggleOneClick(OneClickType.ModelSaber, Settings.CurrentSettings.ModelSaber.OneClickInstaller);
        }

        private void ButtonRestartAsAdmin_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RestartAsAdmin();
        }
    }
}
