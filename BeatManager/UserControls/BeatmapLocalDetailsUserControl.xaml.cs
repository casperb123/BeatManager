using BeatManager.Entities;
using BeatManager.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeatManager.UserControls
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

            dataGridDifficultyRequirements.SelectionChanged += (s, e) => dataGridDifficultyRequirements.UnselectAll();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Back();
        }

        private void ButtonDeleteBeatmap_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteSong();
        }

        private void ButtonRefreshData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshData();
        }

        private void ButtonOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFolder();
        }

        private void ButtonPreviewBeatmap_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PreviewBeatmap();
        }

        private void ButtonOpenOnBeatsaver_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenBeatSaver();
        }

        private void ButtonInvalid_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowErrors();
        }

        private void DataGridDifficultyRequirements_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            string modName = e.Row.Item.ToString().Replace(" ", "");
            SupportedMod supportedMod = App.SupportedMods.FirstOrDefault(x => x.Name == modName);

            if (supportedMod is null)
                e.Row.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            else
            {
                if (supportedMod.Supported == 1)
                    e.Row.Foreground = new SolidColorBrush(Color.FromRgb(214, 143, 0));
                else if (supportedMod.Supported == 2)
                    e.Row.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0));
            }
        }
    }
}
