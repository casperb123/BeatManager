using BeatManager.ViewModels.Download;
using ModelSaber.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls.Download
{
    /// <summary>
    /// Interaction logic for SaberCompletedUserControl.xaml
    /// </summary>
    public partial class ModelSaberCompletedUserControl : UserControl
    {
        public readonly ModelSaberCompletedUserControlViewModel ViewModel;

        public ModelSaberCompletedUserControl(OnlineModel model, string downloaded)
        {
            InitializeComponent();
            ViewModel = new ModelSaberCompletedUserControlViewModel(model, downloaded);
            DataContext = ViewModel;
        }
    }
}
