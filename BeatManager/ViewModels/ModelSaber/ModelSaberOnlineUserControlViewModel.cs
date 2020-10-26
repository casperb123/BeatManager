using BeatManager.UserControls.ModelSaber;
using MahApps.Metro.Controls.Dialogs;
using ModelSaber.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BeatManager.ViewModels.ModelSaber
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

        public List<Sort> SortTypes { get; private set; }
        public List<Filter> Filters { get; private set; }
        public List<FilterType> FilterTypes { get; private set; }

        public MainWindow MainWindow { get; private set; }
        public bool ModelChanged { get; private set; }
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

        public ModelSaberOnlineUserControlViewModel(MainWindow mainWindow, ModelSaberOnlineUserControl userControl)
        {
            MainWindow = mainWindow;
            this.userControl = userControl;

            SelectedModels = new List<OnlineModel>();
            SortTypes = Enum.GetValues(typeof(Sort)).Cast<Sort>().ToList();
            Filters = new List<Filter>();
            FilterTypes = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().ToList();
        }

        public void GetSabers(int page = 0)
        {
            string sortDirection = SortDescending ? "Descending" : "Ascending";
            MainWindow.ToggleLoading(true, "Loading online sabers", $"Sorting by: {CurrentSort} - {sortDirection}");

            _ = Task.Run(async () =>
            {
                try
                {
                    OnlineModels = await App.ModelSaberApi.GetOnlineSabers(CurrentSort, SortDescending, Filters, page);
                }
                catch (Exception e)
                {
                    string description = e.Message;
                    if (e.InnerException != null && !e.Message.Contains(e.InnerException.Message))
                        description += $" ({e.InnerException.Message})";

                    await MainWindow.ShowMessageAsync("Online Sabers", description);
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
            UpdateModels();
        }

        public void PreviousPage()
        {
            App.ModelSaberApi.ChangeOnlinePage(OnlineModels, OnlineModels.PrevPage.Value);
            UpdateModels();
        }

        public void FirstPage()
        {
            App.ModelSaberApi.ChangeOnlinePage(OnlineModels, 0);
            UpdateModels();
        }

        public void LastPage()
        {
            App.ModelSaberApi.ChangeOnlinePage(OnlineModels, OnlineModels.LastPage);
            UpdateModels();
        }

        private void UpdateModels()
        {
            userControl.dataGridModels.UnselectAll();
            userControl.dataGridModels.Items.Refresh();
            UpdatePageButtons();
        }

        public void ChangeSortDirection()
        {
            SortDescending = !SortDescending;
            GetSabers();
        }

        public void AddFilter(Filter filter)
        {
            Filters.Add(filter);
            ModelSaberOnlineFilterUserControl filterUserControl = new ModelSaberOnlineFilterUserControl(filter);
            filterUserControl.RemoveEvent += (s, e) => RemoveFilter(filterUserControl);
            userControl.stackPanelFilters.Children.Add(filterUserControl);
            userControl.textBoxFilterSearch.Clear();

            GetSabers();
        }

        public void RemoveFilter(ModelSaberOnlineFilterUserControl filterUserControl)
        {
            Filters.Remove(filterUserControl.Filter);
            userControl.stackPanelFilters.Children.Remove(filterUserControl);

            GetSabers();
        }
    }
}
