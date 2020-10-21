using BeatManager.ViewModels;
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
    public partial class SaberModelOnlineUserControl : UserControl
    {
        public readonly SaberModelOnlineUserControlViewModel ViewModel;

        public SaberModelOnlineUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new SaberModelOnlineUserControlViewModel(mainWindow, this);
            DataContext = ViewModel;
        }
    }
}
