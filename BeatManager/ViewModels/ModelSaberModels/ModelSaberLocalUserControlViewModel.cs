using BeatManager.UserControls.ModelSaber;
using MahApps.Metro.Controls.Dialogs;
using ModelSaber.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BeatManager.ViewModels.ModelSaberModels
{
    public class ModelSaberLocalUserControlViewModel : INotifyPropertyChanged
    {
        private readonly ModelSaberLocalUserControl userControl;
        private LocalModels localModels;

        private int selectedModelsToDownload;
        private int selectedModelsToDelete;

        public MainWindow MainWindow { get; private set; }
        public bool ModelChanged { get; set; }
        public bool IsLoaded { get; set; }
        public ModelType ModelType { get; private set; }

        public List<LocalModel> SelectedModels { get; set; }
        public int SelectedModelsToDownload
        {
            get { return selectedModelsToDownload; }
            set
            {
                selectedModelsToDownload = value;
                OnPropertyChanged(nameof(SelectedModelsToDownload));
            }
        }
        public int SelectedModelsToDelete
        {
            get { return selectedModelsToDelete; }
            set
            {
                selectedModelsToDelete = value;
                OnPropertyChanged(nameof(SelectedModelsToDelete));
            }
        }

        public LocalModels LocalModels
        {
            get { return localModels; }
            set
            {
                localModels = value;
                OnPropertyChanged(nameof(LocalModels));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ModelSaberLocalUserControlViewModel(MainWindow mainWindow, ModelSaberLocalUserControl userControl, ModelType modelType)
        {
            MainWindow = mainWindow;
            this.userControl = userControl;
            ModelType = modelType;
            SelectedModels = new List<LocalModel>();
        }

        public void GetModels(LocalModels localModels = null)
        {
            MainWindow.ToggleLoading(true, $"Loading local {ModelType.ToString().ToLower()}s");

            _ = Task.Run(async () =>
            {
                try
                {
                    LocalModels = App.ModelSaberApi.GetLocalModels(ModelType, localModels);
                }
                catch (Exception e)
                {
                    string description = e.Message;
                    if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                        description += $" ({e.InnerException.Message})";

                    await MainWindow.ShowMessageAsync("Local Sabers", description);
                }
            });
        }

        public void UpdatePageButtons()
        {
            if (LocalModels is null)
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
                userControl.buttonNextPage.IsEnabled = false;
                userControl.buttonLastPage.IsEnabled = false;
                return;
            }

            if (LocalModels != null && LocalModels.PrevPage.HasValue)
            {
                userControl.buttonFirstPage.IsEnabled = true;
                userControl.buttonPreviousPage.IsEnabled = true;
            }
            else
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
            }
            if (LocalModels != null && LocalModels.NextPage.HasValue)
            {
                userControl.buttonNextPage.IsEnabled = true;
                userControl.buttonLastPage.IsEnabled = true;
            }
            else
            {
                userControl.buttonNextPage.IsEnabled = false;
                userControl.buttonLastPage.IsEnabled = false;
            }
        }

        public void NextPage()
        {
            App.ModelSaberApi.ChangeLocalPage(LocalModels, LocalModels.NextPage.Value);
            UpdateModels();
        }

        public void PreviousPage()
        {
            App.ModelSaberApi.ChangeLocalPage(LocalModels, LocalModels.PrevPage.Value);
            UpdateModels();
        }

        public void FirstPage()
        {
            App.ModelSaberApi.ChangeLocalPage(LocalModels, 0);
            UpdateModels();
        }

        public void LastPage()
        {
            App.ModelSaberApi.ChangeLocalPage(LocalModels, LocalModels.LastPage);
            UpdateModels();
        }

        private void UpdateModels()
        {
            userControl.dataGridModels.UnselectAll();
            userControl.dataGridModels.Items.Refresh();
            UpdatePageButtons();
        }

        public void DeleteModel(LocalModel model)
        {
            ModelSaberOnlineUserControl onlineUserControl = PropertyHelper.GetPropValue<ModelSaberOnlineUserControl>(MainWindow.ViewModel, $"{ModelType}OnlineUserControl");
            OnlineModel onlineModel = onlineUserControl.ViewModel.OnlineModels?.Models.FirstOrDefault(x => x.Id == model.Id || x.Name == model.Name && model.Id == -1);

            App.ModelSaberApi.DeleteModel(model);
            LocalModels.Models.Remove(model);
            if (onlineModel is null)
                ModelChanged = true;
            else
                onlineModel.IsDownloaded = false;

            LocalModels = App.ModelSaberApi.RefreshLocalPages(LocalModels);
        }

        public void DeleteModels(List<LocalModel> models)
        {
            ModelSaberOnlineUserControl onlineUserControl = PropertyHelper.GetPropValue<ModelSaberOnlineUserControl>(MainWindow.ViewModel, $"{ModelType}OnlineUserControl");
            List<OnlineModel> onlineModels = new List<OnlineModel>();

            foreach (LocalModel model in models)
            {
                LocalModels.Models.Remove(model);
                App.ModelSaberApi.DeleteModel(model);

                OnlineModel onlineModel = onlineUserControl.ViewModel.OnlineModels?.Models.FirstOrDefault(x => x.Id == model.Id || x.Name == model.Name && model.Id == -1);

                if (onlineModel != null)
                    onlineModels.Add(onlineModel);
            }

            if (onlineModels.Count == models.Count)
                onlineModels.ForEach(x => x.IsDownloaded = false);
            else
                ModelChanged = true;

            LocalModels = App.ModelSaberApi.RefreshLocalPages(LocalModels);
        }

        public void ModelDetails(LocalModel model, bool changePage = true)
        {
            MainWindow.ViewModel.ModelSaberLocalDetailsUserControl.ViewModel.Model = model;
            if (changePage)
                MainWindow.userControlMain.Content = MainWindow.ViewModel.ModelSaberLocalDetailsUserControl;

            userControl.dataGridModels.UnselectAll();
        }

        public void OpenBigCover(ImageSource image)
        {
            MainWindow.ViewModel.OpenBigCover(image);
        }
    }
}
