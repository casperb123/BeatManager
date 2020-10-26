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
    public partial class SaberOnlineUserControl : UserControl
    {
        public readonly ModelSaberOnlineUserControl SaberUserControl;

        public SaberOnlineUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            SaberUserControl = new ModelSaberOnlineUserControl(mainWindow);
            userControlMain.Content = SaberUserControl;
        }
    }
}
