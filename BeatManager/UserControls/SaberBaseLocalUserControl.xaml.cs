using BeatManager.UserControls.ModelSaber;
using System.Windows.Controls;

namespace BeatManager.UserControls
{
    /// <summary>
    /// Interaction logic for SaberLocalUserControl.xaml
    /// </summary>
    public partial class SaberBaseLocalUserControl : UserControl
    {
        public readonly SaberLocalUserControl UserControl;

        public SaberBaseLocalUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            UserControl = new SaberLocalUserControl(mainWindow);
            userControlMain.Content = UserControl;
        }
    }
}
