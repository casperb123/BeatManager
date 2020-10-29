using BeatManager.ViewModels.Download;
using ModelSaber.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls.Download
{
    /// <summary>
    /// Interaction logic for SaberDownloadingUserControl.xaml
    /// </summary>
    public partial class ModelSaberDownloadingUserControl : UserControl
    {
        public readonly ModelSaberDownloadingUserControlViewModel ViewModel;

        public ModelSaberDownloadingUserControl(OnlineModel model)
        {
            InitializeComponent();
            ViewModel = new ModelSaberDownloadingUserControlViewModel(model);
            DataContext = ViewModel;
        }
    }
}
