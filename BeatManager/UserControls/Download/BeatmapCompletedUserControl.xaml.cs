using BeatManager.ViewModels.Download;
using BeatSaver.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls.Download
{
    /// <summary>
    /// Interaction logic for OnlineBeatmapCompletedUserControl.xaml
    /// </summary>
    public partial class BeatmapCompletedUserControl : UserControl
    {
        public readonly BeatmapCompletedUserControlViewModel ViewModel;

        public BeatmapCompletedUserControl(OnlineBeatmap beatmap, string downloaded)
        {
            InitializeComponent();
            ViewModel = new BeatmapCompletedUserControlViewModel(beatmap);
            DataContext = ViewModel;
        }
    }
}
