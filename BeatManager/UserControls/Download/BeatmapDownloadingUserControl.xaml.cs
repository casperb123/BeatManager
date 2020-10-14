using BeatManager.ViewModels.Download;
using BeatSaver.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls.Download
{
    /// <summary>
    /// Interaction logic for OnlineBeatmapDownload.xaml
    /// </summary>
    public partial class BeatmapDownloadingUserControl : UserControl
    {
        public readonly BeatmapDownloadingUserControlViewModel ViewModel;

        public BeatmapDownloadingUserControl(OnlineBeatmap beatmap)
        {
            InitializeComponent();
            ViewModel = new BeatmapDownloadingUserControlViewModel(beatmap);
            DataContext = ViewModel;
        }
    }
}
