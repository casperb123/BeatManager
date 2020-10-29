using BeatManager.UserControls.ModelSaber;
using MahApps.Metro.Controls.Dialogs;
using ModelSaber.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BeatManager.ViewModels.ModelSaberModels
{
    public class ModelSaberLocalUserControlViewModel : INotifyPropertyChanged
    {
        private readonly ModelSaberLocalUserControl userControl;
        private LocalModels localModels;
        private readonly ModelType modelType;

        private int selectedModelsToDownload;
        private int selectedModelsToDelete;

        public MainWindow MainWindow { get; private set; }
        public bool ModelChanged { get; private set; }
        public bool IsLoaded { get; set; }

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
            this.modelType = modelType;

            SelectedModels = new List<LocalModel>();
        }

        public void GetSabers(LocalModels localModels = null)
        {
            MainWindow.ToggleLoading(true, "Loading online sabers");

            _ = Task.Run(async () =>
            {
                try
                {
                    LocalModels = App.ModelSaberApi.GetLocalModels(modelType, localModels);
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

        public void DeleteModel(string name)
        {
            LocalModel localModel = LocalModels.Models.FirstOrDefault(x => x.Name == name);
            OnlineModel onlineModel = MainWindow.ViewModel.SaberOnlineUserControl.UserControl.ViewModel.OnlineModels?.Models.FirstOrDefault(x => x.Name == name);

            App.ModelSaberApi.DeleteModel(localModel);
            LocalModels.Models.Remove(localModel);
            if (onlineModel is null)
                TriggerChange();
            else
                onlineModel.IsDownloaded = false;

            LocalModels = App.ModelSaberApi.RefreshLocalPages(LocalModels);
        }

        public void DeleteModels(List<LocalModel> models)
        {
            List<OnlineModel> onlineModels = new List<OnlineModel>();

            foreach (LocalModel model in models)
            {
                LocalModels.Models.Remove(model);
                App.ModelSaberApi.DeleteModel(model);

                OnlineModel onlineModel = MainWindow.ViewModel.SaberOnlineUserControl.UserControl.ViewModel.OnlineModels?.Models.FirstOrDefault(x => x.Name == model.Name);
                if (onlineModel != null)
                    onlineModels.Add(onlineModel);
            }

            if (onlineModels.Count == models.Count)
                onlineModels.ForEach(x => x.IsDownloaded = false);
            else
                TriggerChange();

            LocalModels = App.ModelSaberApi.RefreshLocalPages(LocalModels);
        }

        private void TriggerChange()
        {
            switch (modelType)
            {
                case ModelType.None:
                    break;
                case ModelType.Saber:
                    MainWindow.ViewModel.LocalSaberChanged = true;
                    break;
                case ModelType.Avatar:
                    MainWindow.ViewModel.LocalAvatarChanged = true;
                    break;
                case ModelType.Platform:
                    MainWindow.ViewModel.LocalPlatformChanged = true;
                    break;
                case ModelType.Bloq:
                    MainWindow.ViewModel.LocalBloqChanged = true;
                    break;
                default:
                    break;
            }
        }
    }
}
