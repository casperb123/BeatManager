using BeatManager.UserControls.ModelSaber;
using ModelSaber.Entities;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BeatManager.ViewModels.ModelSaberModels
{
    public class ModelSaberOnlineDetailsUserControlViewModel : INotifyPropertyChanged
    {
        private readonly ModelSaberOnlineDetailsUserControl userControl;
        private readonly MainWindow mainWindow;
        private OnlineModel model;
        private BitmapImage coverImage;

        public BitmapImage CoverImage
        {
            get { return coverImage; }
            set
            {
                coverImage = value;
                OnPropertyChanged(nameof(CoverImage));
            }
        }

        public OnlineModel Model
        {
            get { return model; }
            set
            {
                model = value;
                OnPropertyChanged(nameof(Model));
                CoverImage = new BitmapImage(new Uri(value.RealThumbnail));
                CreateTags();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ModelSaberOnlineDetailsUserControlViewModel(ModelSaberOnlineDetailsUserControl userControl, MainWindow mainWindow)
        {
            this.userControl = userControl;
            this.mainWindow = mainWindow;
        }

        private void CreateTags()
        {
            userControl.wrapPanelTags.Children.Clear();
            //Model.Tags.ForEach(x => userControl.wrapPanelTags.Children.Add(new ModelSaberTagUserControl(x)));
            for (int i = 0; i < 50; i++)
            {
                userControl.wrapPanelTags.Children.Add(new ModelSaberTagUserControl($"Test{i}"));
            }
        }

        public void Back()
        {
            ModelSaberOnlineUserControl onlineUserControl = PropertyHelper.GetPropValue<ModelSaberOnlineUserControl>(mainWindow.ViewModel, $"{Model.ModelType}OnlineUserControl");
            mainWindow.userControlMain.Content = onlineUserControl;
        }

        public async Task DownloadModel()
        {
            ModelSaberOnlineUserControl onlineUserControl = PropertyHelper.GetPropValue<ModelSaberOnlineUserControl>(mainWindow.ViewModel, $"{Model.ModelType}OnlineUserControl");
            await onlineUserControl.ViewModel.DownloadModel(Model);
        }

        public void DeleteModel()
        {
            ModelSaberOnlineUserControl onlineUserControl = PropertyHelper.GetPropValue<ModelSaberOnlineUserControl>(mainWindow.ViewModel, $"{Model.ModelType}OnlineUserControl");
            onlineUserControl.ViewModel.DeleteModel(Model);
        }

        public void RefreshData()
        {
            ModelSaberOnlineUserControl onlineUserControl = PropertyHelper.GetPropValue<ModelSaberOnlineUserControl>(mainWindow.ViewModel, $"{Model.ModelType}OnlineUserControl");
            onlineUserControl.ViewModel.ModelDetails(Model, false);
        }

        public void OpenFolder()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.GetDirectoryName(Model.ModelPath),
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void OpenModelSaber()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://modelsaber.com/{Model.ModelType}s/?id={Model.Id}",
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void OpenBeatSaberAuthor(string link)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = link,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        public void OpenBigCover()
        {
            mainWindow.ViewModel.OpenBigCover(CoverImage);
        }
    }
}
