using BeatManager.ViewModels.Download;
using ModelSaber.Entities;
using System;
using System.Windows.Controls;

namespace BeatManager.UserControls.Download
{
    /// <summary>
    /// Interaction logic for ModelSaberFailedUserControl.xaml
    /// </summary>
    public partial class ModelSaberFailedUserControl : UserControl
    {
        public readonly ModelSaberFailedUserControlViewModel ViewModel;

        public ModelSaberFailedUserControl(OnlineModel model, Exception exception)
        {
            InitializeComponent();
            string message = exception.Message;
            if (exception.InnerException != null && !exception.Message.Contains(exception.InnerException.Message))
                message += $" ({exception.InnerException.Message})";

            ViewModel = new ModelSaberFailedUserControlViewModel(model, message);
            DataContext = ViewModel;
        }
    }
}
