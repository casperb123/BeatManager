using BeatManager.UserControls.ModelSaber;
using MahApps.Metro.Controls.Dialogs;
using ModelSaber.Entities;
using ModelSaber.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BeatManager.ViewModels.ModelSaberModels
{
    public class ModelSaberOnlineUserControlViewModel : INotifyPropertyChanged
    {
        private readonly ModelSaberOnlineUserControl userControl;
        private OnlineModels onlineModels;

        private int selectedModelsToDownload;
        private int selectedModelsToDelete;
        private Sort currentSort;
        private bool sortDescending;
        private FilterType currentFilterType;

        public ModelType ModelType { get; private set; }
        public List<Sort> SortTypes { get; private set; }
        public List<Filter> Filters { get; private set; }
        public List<FilterType> FilterTypes { get; private set; }

        public MainWindow MainWindow { get; private set; }
        public bool ModelChanged { get; set; }
        public bool IsLoaded { get; set; }

        public FilterType CurrentFilterType
        {
            get { return currentFilterType; }
            set
            {
                currentFilterType = value;
                OnPropertyChanged(nameof(CurrentFilterType));
            }
        }

        public Sort CurrentSort
        {
            get { return currentSort; }
            set
            {
                currentSort = value;
                OnPropertyChanged(nameof(CurrentSort));
            }
        }

        public bool SortDescending
        {
            get { return sortDescending; }
            set
            {
                sortDescending = value;
                OnPropertyChanged(nameof(SortDescending));
            }
        }

        public string CurrentSearchQuery { get; set; }
        public List<OnlineModel> SelectedModels { get; set; }
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

        public OnlineModels OnlineModels
        {
            get { return onlineModels; }
            set
            {
                onlineModels = value;
                OnPropertyChanged(nameof(OnlineModels));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ModelSaberOnlineUserControlViewModel(MainWindow mainWindow, ModelSaberOnlineUserControl userControl, ModelType modelType)
        {
            MainWindow = mainWindow;
            this.userControl = userControl;
            ModelType = modelType;

            SelectedModels = new List<OnlineModel>();
            SortTypes = Enum.GetValues(typeof(Sort)).Cast<Sort>().ToList();
            Filters = new List<Filter>();
            FilterTypes = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().ToList();

            App.ModelSaberApi.DownloadCompleted += ModelSaberApi_DownloadCompleted;
        }

        private void ModelSaberApi_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if (OnlineModels is null || e.Model.ModelType != ModelType)
                return;

            if (!OnlineModels.Models.Contains(e.Model))
            {
                OnlineModel onlineModel = OnlineModels.Models.FirstOrDefault(x => x.Id == e.Model.Id);
                if (onlineModel != null)
                {
                    onlineModel.IsDownloading = e.Model.IsDownloading;
                    onlineModel.IsDownloaded = e.Model.IsDownloaded;
                }
            }

            if (!OnlineModels.Models.Any(x => x.IsDownloading) && !App.ModelSaberApi.Downloading.Any())
                MainWindow.radioButtonSettings.IsEnabled = true;

            ModelChanged = true;
            UpdatePageButtons();
        }

        public void GetModels(OnlineModels onlineModels = null)
        {
            string sortDirection = SortDescending ? "Descending" : "Ascending";
            MainWindow.ToggleLoading(true, $"Loading online {ModelType.ToString().ToLower()}s", $"Sorting by: {CurrentSort} - {sortDirection}");

            _ = Task.Run(async () =>
            {
                try
                {
                    OnlineModels = await App.ModelSaberApi.GetOnlineModels(ModelType, CurrentSort, SortDescending, Filters, onlineModels);
                }
                catch (Exception e)
                {
                    string description = e.Message;
                    if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                        description += $" ({e.InnerException.Message})";

                    await MainWindow.ShowMessageAsync($"Online {ModelType}s", description);
                }
            });
        }

        public void UpdatePageButtons()
        {
            if (OnlineModels is null)
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
                userControl.buttonNextPage.IsEnabled = false;
                userControl.buttonLastPage.IsEnabled = false;
                return;
            }

            if (OnlineModels != null && OnlineModels.PrevPage.HasValue)
            {
                userControl.buttonFirstPage.IsEnabled = true;
                userControl.buttonPreviousPage.IsEnabled = true;
            }
            else
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
            }
            if (OnlineModels != null && OnlineModels.NextPage.HasValue)
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
            App.ModelSaberApi.ChangeOnlinePage(OnlineModels, OnlineModels.NextPage.Value);
            GetModels(OnlineModels);
        }

        public void PreviousPage()
        {
            App.ModelSaberApi.ChangeOnlinePage(OnlineModels, OnlineModels.PrevPage.Value);
            GetModels(OnlineModels);
        }

        public void FirstPage()
        {
            App.ModelSaberApi.ChangeOnlinePage(OnlineModels, 0);
            GetModels(OnlineModels);
        }

        public void LastPage()
        {

            App.ModelSaberApi.ChangeOnlinePage(OnlineModels, OnlineModels.LastPage);
            GetModels(OnlineModels);
        }

        public void ChangeSortDirection()
        {
            SortDescending = !SortDescending;
            GetModels();
        }

        public void AddFilter(Filter filter)
        {
            Filters.Add(filter);
            ModelSaberOnlineFilterUserControl filterUserControl = new ModelSaberOnlineFilterUserControl(filter);
            filterUserControl.RemoveEvent += (s, e) => RemoveFilter(filterUserControl);
            userControl.wrapPanelFilters.Children.Add(filterUserControl);
            userControl.textBoxFilterSearch.Clear();

            GetModels();
        }

        public void RemoveFilter(ModelSaberOnlineFilterUserControl filterUserControl)
        {
            Filters.Remove(filterUserControl.Filter);
            userControl.wrapPanelFilters.Children.Remove(filterUserControl);

            GetModels();
        }

        public async Task DownloadModel(OnlineModel model)
        {
            try
            {
                MainWindow.radioButtonSettings.IsEnabled = false;
                await App.ModelSaberApi.DownloadModel(model);
            }
            catch (InvalidOperationException e)
            {
                MainWindow.ToggleLoading(false);
                string errorMessage = e.Message;
                if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                    errorMessage += $" ({e.InnerException.Message})";

                await MainWindow.ShowMessageAsync("Downloading failed", "Downloading the model failed with the following error\n\n" +
                                                                        "Error:\n" +
                                                                        $"{errorMessage}");
            }
            catch (Exception e)
            {
                MainWindow.ToggleLoading(false);
                string errorMessage = e.Message;
                if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                    errorMessage += $" ({e.InnerException.Message})";

                MessageDialogResult result = await MainWindow.ShowMessageAsync("Downloading failed", "Downloading the model failed, would you like to try again?\n\n" +
                                                                                                     "Error:\n" +
                                                                                                     $"{errorMessage}", MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                    await DownloadModel(model);
            }
        }

        public async Task DownloadModels(List<OnlineModel> models)
        {
            MainWindow.radioButtonSettings.IsEnabled = false;

            foreach (OnlineModel model in models)
            {
                try
                {
                    _ = App.ModelSaberApi.DownloadModel(model).ConfigureAwait(false);
                }
                catch (InvalidOperationException e)
                {
                    MainWindow.ToggleLoading(false);
                    string errorMessage = e.Message;
                    if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                        errorMessage += $" ({e.InnerException.Message})";

                    await MainWindow.ShowMessageAsync("Downloading failed", "Downloading the model failed with the following error\n\n" +
                                                                            "Error:\n" +
                                                                            $"{errorMessage}");
                }
                catch (Exception e)
                {
                    MainWindow.ToggleLoading(false);
                    string errorMessage = e.Message;
                    if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                        errorMessage += $" ({e.InnerException.Message})";

                    MessageDialogResult result = await MainWindow.ShowMessageAsync("Downloading failed", "Downloading the model failed, would you like to try again?\n\n" +
                                                                                                         "Error:\n" +
                                                                                                         $"{errorMessage}", MessageDialogStyle.AffirmativeAndNegative);

                    if (result == MessageDialogResult.Affirmative)
                        await DownloadModel(model);
                }
            }
        }

        public void DeleteModel(OnlineModel model)
        {
            ModelSaberLocalUserControl localUserControl = PropertyHelper.GetPropValue<ModelSaberLocalUserControl>(MainWindow.ViewModel, $"{ModelType}LocalUserControl");
            LocalModel localModel = localUserControl.ViewModel.LocalModels?.Models.FirstOrDefault(x => x.Id == model.Id || x.Name == model.Name && x.Id == -1);

            if (localModel != null)
                localUserControl.ViewModel.DeleteModel(localModel);

            App.ModelSaberApi.DeleteModel(model);
            ModelChanged = true;
        }

        public void DeleteModels(List<OnlineModel> models)
        {
            ModelSaberLocalUserControl localUserControl = PropertyHelper.GetPropValue<ModelSaberLocalUserControl>(MainWindow.ViewModel, $"{ModelType}LocalUserControl");

            foreach (OnlineModel model in models)
            {
                LocalModel localModel = localUserControl.ViewModel.LocalModels?.Models.FirstOrDefault(x => x.Id == model.Id || x.Name == model.Name && x.Id == -1);

                if (localModel != null)
                    localUserControl.ViewModel.DeleteModel(localModel);

                App.ModelSaberApi.DeleteModel(model);
            }

            if (models.Count > 0)
                ModelChanged = true;
        }

        public void ModelDetails(OnlineModel model, bool changePage = true)
        {
            MainWindow.ViewModel.ModelSaberOnlineDetailsUserControl.ViewModel.Model = model;
            if (changePage)
                MainWindow.userControlMain.Content = MainWindow.ViewModel.ModelSaberOnlineDetailsUserControl;

            userControl.dataGridModels.UnselectAll();
        }

        public void OpenBigCover(OnlineModel model)
        {
            BitmapImage image = new BitmapImage(new Uri(model.RealThumbnail));
            MainWindow.ViewModel.OpenBigCover(image);
        }
    }
}
