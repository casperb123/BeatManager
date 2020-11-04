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
                CoverImage = new BitmapImage(new Uri(Model.RealThumbnail));
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
            Model.Tags.ForEach(x => userControl.wrapPanelTags.Children.Add(new ModelSaberTagUserControl(x)));
        }

        public void Back()
        {
            switch (Model.ModelType)
            {
                case ModelType.None:
                    break;
                case ModelType.Saber:
                    mainWindow.userControlMain.Content = mainWindow.ViewModel.SaberOnlineUserControl;
                    break;
                case ModelType.Avatar:
                    mainWindow.userControlMain.Content = mainWindow.ViewModel.AvatarOnlineUserControl;
                    break;
                case ModelType.Platform:
                    mainWindow.userControlMain.Content = mainWindow.ViewModel.PlatformOnlineUserControl;
                    break;
                case ModelType.Bloq:
                    mainWindow.userControlMain.Content = mainWindow.ViewModel.BloqOnlineUserControl;
                    break;
                default:
                    break;
            }
        }

        public async Task DownloadModel()
        {
            switch (Model.ModelType)
            {
                case ModelType.None:
                    break;
                case ModelType.Saber:
                    await mainWindow.ViewModel.SaberOnlineUserControl.ViewModel.DownloadModel(Model.Id);
                    break;
                case ModelType.Avatar:
                    await mainWindow.ViewModel.AvatarOnlineUserControl.ViewModel.DownloadModel(Model.Id);
                    break;
                case ModelType.Platform:
                    await mainWindow.ViewModel.PlatformOnlineUserControl.ViewModel.DownloadModel(Model.Id);
                    break;
                case ModelType.Bloq:
                    await mainWindow.ViewModel.BloqOnlineUserControl.ViewModel.DownloadModel(Model.Id);
                    break;
                default:
                    break;
            }
        }

        public void DeleteModel()
        {
            switch (Model.ModelType)
            {
                case ModelType.None:
                    break;
                case ModelType.Saber:
                    mainWindow.ViewModel.SaberOnlineUserControl.ViewModel.DeleteModel(Model.Id);
                    break;
                case ModelType.Avatar:
                    mainWindow.ViewModel.AvatarOnlineUserControl.ViewModel.DeleteModel(Model.Id);
                    break;
                case ModelType.Platform:
                    mainWindow.ViewModel.PlatformOnlineUserControl.ViewModel.DeleteModel(Model.Id);
                    break;
                case ModelType.Bloq:
                    mainWindow.ViewModel.BloqOnlineUserControl.ViewModel.DeleteModel(Model.Id);
                    break;
                default:
                    break;
            }
        }

        public void RefreshData()
        {
            switch (Model.ModelType)
            {
                case ModelType.None:
                    break;
                case ModelType.Saber:
                    mainWindow.ViewModel.SaberOnlineUserControl.ViewModel.ModelDetails(Model.Id, false);
                    break;
                case ModelType.Avatar:
                    mainWindow.ViewModel.AvatarOnlineUserControl.ViewModel.ModelDetails(Model.Id, false);
                    break;
                case ModelType.Platform:
                    mainWindow.ViewModel.PlatformOnlineUserControl.ViewModel.ModelDetails(Model.Id, false);
                    break;
                case ModelType.Bloq:
                    mainWindow.ViewModel.BloqOnlineUserControl.ViewModel.ModelDetails(Model.Id, false);
                    break;
                default:
                    break;
            }
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

        public void OpenBigCover()
        {
            mainWindow.ViewModel.OpenBigCover(CoverImage);
        }
    }
}
