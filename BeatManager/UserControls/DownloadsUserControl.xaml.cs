using BeatManager.ViewModels;
using System.Windows.Controls;

namespace BeatManager.UserControls
{
    /// <summary>
    /// Interaction logic for DownloadsUserControl.xaml
    /// </summary>
    public partial class DownloadsUserControl : UserControl
    {
        public readonly DownloadsUserControlViewModel ViewModel;

        public DownloadsUserControl()
        {
            InitializeComponent();
            ViewModel = new DownloadsUserControlViewModel();
            DataContext = ViewModel;
        }
    }
}
