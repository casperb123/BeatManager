using BeatManager.ViewModels.Download;
using BeatSaverApi.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls.Download
{
    /// <summary>
    /// Interaction logic for OnlineBeatmapCompletedUserControl.xaml
    /// </summary>
    public partial class OnlineBeatmapCompletedUserControl : UserControl
    {
        public readonly OnlineBeatmapCompletedUserControlViewModel ViewModel;

        public OnlineBeatmapCompletedUserControl(OnlineBeatmap beatmap, string downloaded)
        {
            InitializeComponent();
            ViewModel = new OnlineBeatmapCompletedUserControlViewModel(beatmap);
            DataContext = ViewModel;
        }
    }
}
