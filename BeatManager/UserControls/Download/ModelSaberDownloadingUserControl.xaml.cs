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
    /// Interaction logic for SaberDownloadingUserControl.xaml
    /// </summary>
    public partial class ModelSaberDownloadingUserControl : UserControl
    {
        public readonly ModelSaberDownloadingUserControlViewModel ViewModel;

        public ModelSaberDownloadingUserControl(OnlineModel model)
        {
            InitializeComponent();
            ViewModel = new ModelSaberDownloadingUserControlViewModel(model);
            DataContext = ViewModel;
        }
    }
}
