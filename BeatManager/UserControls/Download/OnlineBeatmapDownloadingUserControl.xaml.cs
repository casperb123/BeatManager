using BeatManager.ViewModels.Download;
using BeatSaverApi.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls.Download
{
    /// <summary>
    /// Interaction logic for OnlineBeatmapDownload.xaml
    /// </summary>
    public partial class OnlineBeatmapDownloadingUserControl : UserControl
    {
        public readonly OnlineBeatmapDownloadingUserControlViewModel ViewModel;

        public OnlineBeatmapDownloadingUserControl(OnlineBeatmap beatmap)
        {
            InitializeComponent();
            ViewModel = new OnlineBeatmapDownloadingUserControlViewModel(beatmap);
            DataContext = ViewModel;
        }
    }
}
