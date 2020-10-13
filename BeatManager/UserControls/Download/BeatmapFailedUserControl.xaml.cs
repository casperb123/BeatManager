using BeatManager.ViewModels.Download;
using BeatSaverApi.Entities;
using System;
using System.Windows.Controls;

namespace BeatManager.UserControls.Download
{
    /// <summary>
    /// Interaction logic for BeatmapFailedUserControl.xaml
    /// </summary>
    public partial class BeatmapFailedUserControl : UserControl
    {
        public readonly BeatmapFailedUserControlViewModel ViewModel;

        public BeatmapFailedUserControl(OnlineBeatmap beatmap, Exception exception)
        {
            InitializeComponent();
            string message = exception.Message;
            if (exception.InnerException != null && !exception.Message.Contains(exception.InnerException.Message))
                message += $" ({exception.InnerException.Message})";

            ViewModel = new BeatmapFailedUserControlViewModel(beatmap, message);
            DataContext = ViewModel;
        }
    }
}
