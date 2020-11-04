using BeatManager.UserControls.ModelSaber;
using ModelSaber.Entities;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace BeatManager.ViewModels.ModelSaberModels
{
    public class ModelSaberLocalDetailsUserControlViewModel : INotifyPropertyChanged
    {
        private readonly ModelSaberLocalDetailsUserControl userControl;
        private readonly MainWindow mainWindow;
        private LocalModel model;
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

        public LocalModel Model
        {
            get { return model; }
            set
            {
                model = value;
                OnPropertyChanged(nameof(Model));
                if (Model.OnlineModel != null)
                {
                    CoverImage = new BitmapImage(new Uri(Model.OnlineModel.RealThumbnail));
                    CreateTags();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ModelSaberLocalDetailsUserControlViewModel(ModelSaberLocalDetailsUserControl userControl, MainWindow mainWindow)
        {
            this.userControl = userControl;
            this.mainWindow = mainWindow;
        }

        private void CreateTags()
        {
            userControl.wrapPanelTags.Children.Clear();
            Model.OnlineModel.Tags.ForEach(x => userControl.wrapPanelTags.Children.Add(new ModelSaberTagUserControl(x)));
        }

        public void Back()
        {
            switch (Model.ModelType)
            {
                case ModelType.None:
                    break;
                case ModelType.Saber:
                    mainWindow.userControlMain.Content = mainWindow.ViewModel.SaberLocalUserControl;
                    break;
                case ModelType.Avatar:
                    mainWindow.userControlMain.Content = mainWindow.ViewModel.AvatarLocalUserControl;
                    break;
                case ModelType.Platform:
                    mainWindow.userControlMain.Content = mainWindow.ViewModel.PlatformLocalUserControl;
                    break;
                case ModelType.Bloq:
                    mainWindow.userControlMain.Content = mainWindow.ViewModel.BloqLocalUserControl;
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
                    mainWindow.ViewModel.SaberLocalUserControl.ViewModel.DeleteModel(Model.Name);
                    break;
                case ModelType.Avatar:
                    mainWindow.ViewModel.AvatarLocalUserControl.ViewModel.DeleteModel(Model.Name);
                    break;
                case ModelType.Platform:
                    mainWindow.ViewModel.PlatformLocalUserControl.ViewModel.DeleteModel(Model.Name);
                    break;
                case ModelType.Bloq:
                    mainWindow.ViewModel.BloqLocalUserControl.ViewModel.DeleteModel(Model.Name);
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
                    mainWindow.ViewModel.SaberLocalUserControl.ViewModel.ModelDetails(Model.Name, false);
                    break;
                case ModelType.Avatar:
                    mainWindow.ViewModel.AvatarLocalUserControl.ViewModel.ModelDetails(Model.Name, false);
                    break;
                case ModelType.Platform:
                    mainWindow.ViewModel.PlatformLocalUserControl.ViewModel.ModelDetails(Model.Name, false);
                    break;
                case ModelType.Bloq:
                    mainWindow.ViewModel.BloqLocalUserControl.ViewModel.ModelDetails(Model.Name, false);
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
                FileName = $"https://modelsaber.com/{Model.ModelType}s/?id={Model.OnlineModel.Id}",
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
