using BeatManager.UserControls.ModelSaber;
using BeatManager.ViewModels;
using ModelSaber.Entities;
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
    /// Interaction logic for ModelOnlineUserControl.xaml
    /// </summary>
    public partial class SaberBaseOnlineUserControl : UserControl
    {
        public readonly SaberOnlineUserControl SaberUserControl;

        public SaberBaseOnlineUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            SaberUserControl = new SaberOnlineUserControl(mainWindow, ModelType.Saber);
            userControlMain.Content = SaberUserControl;
        }
    }
}
