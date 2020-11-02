using ModelSaber.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for ModelOnlineUserControl.xaml
    /// </summary>
    public partial class ModelSaberBaseOnlineUserControl : UserControl
    {
        public readonly ModelSaberOnlineUserControl UserControl;

        public ModelSaberBaseOnlineUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            UserControl = new ModelSaberOnlineUserControl(mainWindow, ModelType.Saber);
            userControlMain.Content = UserControl;
        }
    }
}
