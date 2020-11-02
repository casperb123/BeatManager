using ModelSaber.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for SaberLocalUserControl.xaml
    /// </summary>
    public partial class ModelSaberBaseLocalUserControl : UserControl
    {
        public readonly ModelSaberLocalUserControl UserControl;

        public ModelSaberBaseLocalUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            UserControl = new ModelSaberLocalUserControl(mainWindow, ModelType.Saber);
            userControlMain.Content = UserControl;
        }
    }
}
