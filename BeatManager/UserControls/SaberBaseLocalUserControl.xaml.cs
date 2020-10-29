using BeatManager.UserControls.ModelSaber;
using ModelSaber.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls
{
    /// <summary>
    /// Interaction logic for SaberLocalUserControl.xaml
    /// </summary>
    public partial class SaberBaseLocalUserControl : UserControl
    {
        public readonly ModelSaberLocalUserControl UserControl;

        public SaberBaseLocalUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            UserControl = new ModelSaberLocalUserControl(mainWindow, ModelType.Saber);
            userControlMain.Content = UserControl;
        }
    }
}
