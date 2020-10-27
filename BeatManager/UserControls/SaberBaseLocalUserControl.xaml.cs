using BeatManager.UserControls.ModelSaber;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }
    }
}
