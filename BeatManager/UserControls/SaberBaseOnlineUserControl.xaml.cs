using BeatManager.UserControls.ModelSaber;
using ModelSaber.Entities;
using System.Windows.Controls;

namespace BeatManager.UserControls
{
    /// <summary>
    /// Interaction logic for ModelOnlineUserControl.xaml
    /// </summary>
    public partial class SaberBaseOnlineUserControl : UserControl
    {
        public readonly ModelSaberOnlineUserControl UserControl;

        public SaberBaseOnlineUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            UserControl = new ModelSaberOnlineUserControl(mainWindow, ModelType.Saber);
            userControlMain.Content = UserControl;
        }
    }
}
