using BeatManager.ViewModels.Download;
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

namespace BeatManager.UserControls.Download
{
    /// <summary>
    /// Interaction logic for SaberCompletedUserControl.xaml
    /// </summary>
    public partial class ModelSaberCompletedUserControl : UserControl
    {
        public readonly ModelSaberCompletedUserControlViewModel ViewModel;

        public ModelSaberCompletedUserControl(OnlineModel model)
        {
            InitializeComponent();
            ViewModel = new ModelSaberCompletedUserControlViewModel(model);
            DataContext = ViewModel;
        }
    }
}
