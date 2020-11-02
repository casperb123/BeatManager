using BeatManager.ViewModels.ModelSaberModels;
using ModelSaber.Entities;
using System;
using System.Windows.Controls;

namespace BeatManager.UserControls.ModelSaber
{
    /// <summary>
    /// Interaction logic for ModelOnlineUserControl.xaml
    /// </summary>
    public partial class ModelSaberBaseOnlineUserControl : UserControl
    {
        public readonly ModelSaberBaseOnlineUserControlViewModel ViewModel;

        public ModelSaberBaseOnlineUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new ModelSaberBaseOnlineUserControlViewModel(mainWindow, this);
            DataContext = ViewModel;
        }
    }
}
