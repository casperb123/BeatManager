﻿using BeatManager.Entities.ModelSaber;
using BeatManager.UserControls;
using MahApps.Metro.Controls.Dialogs;
using ModelSaber.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace BeatManager.ViewModels
{
    public class SaberOnlineUserControlViewModel : INotifyPropertyChanged
    {
        private readonly SaberOnlineUserControl userControl;
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

        public SaberOnlineUserControlViewModel(MainWindow mainWindow, SaberOnlineUserControl userControl)
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
                    OnlineModels = await App.ModelSaberApi.GetOnlineSaberModels(CurrentSort, SortDescending, Filters, page);
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
                userControl.buttonLastPage.IsEnabled = false;
                userControl.buttonNextPage.IsEnabled = false;
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
                userControl.buttonLastPage.IsEnabled = true;
                userControl.buttonNextPage.IsEnabled = true;
            }
            else
            {
                userControl.buttonLastPage.IsEnabled = false;
                userControl.buttonNextPage.IsEnabled = false;
            }
        }
    }
}
