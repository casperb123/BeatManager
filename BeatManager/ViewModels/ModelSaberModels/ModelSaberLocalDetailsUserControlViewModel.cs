using BeatManager.UserControls.ModelSaber;
using MahApps.Metro.Controls.Dialogs;
using ModelSaber.Entities;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                if (value.OnlineModel != null)
                    CreateTags();
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
            ModelSaberLocalUserControl localUserControl = PropertyHelper.GetPropValue<ModelSaberLocalUserControl>(mainWindow.ViewModel, $"{Model.ModelType}LocalUserControl");
            mainWindow.userControlMain.Content = localUserControl;
        }

        public async Task ShowErrors()
        {
            string errorsText = string.Join("\n", Model.Errors.Select(x => $"- {x}"));
            await mainWindow.ShowMessageAsync($"{Model.ModelType} Invalid", $"The {Model.ModelType.ToString().ToLower()} has the following errors:\n" +
                                                                 $"{errorsText}");
        }

        public void DeleteModel()
        {
            ModelSaberLocalUserControl localUserControl = PropertyHelper.GetPropValue<ModelSaberLocalUserControl>(mainWindow.ViewModel, $"{Model.ModelType}LocalUserControl");
            localUserControl.ViewModel.DeleteModel(Model);
        }

        public void RefreshData()
        {
            ModelSaberLocalUserControl localUserControl = PropertyHelper.GetPropValue<ModelSaberLocalUserControl>(mainWindow.ViewModel, $"{Model.ModelType}LocalUserControl");
            localUserControl.ViewModel.ModelDetails(Model, false);
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

        public void OpenBeastSaberAuthor(string link)
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
