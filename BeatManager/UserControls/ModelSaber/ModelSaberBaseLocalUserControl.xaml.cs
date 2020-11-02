using BeatManager.ViewModels.ModelSaberModels;
using ModelSaber.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for SaberLocalUserControl.xaml
    /// </summary>
    public partial class ModelSaberBaseLocalUserControl : UserControl
    {
        public readonly ModelSaberBaseLocalUserControlViewModel ViewModel;

        public ModelSaberBaseLocalUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new ModelSaberBaseLocalUserControlViewModel(mainWindow, this);
            DataContext = ViewModel;
        }
    }
}
