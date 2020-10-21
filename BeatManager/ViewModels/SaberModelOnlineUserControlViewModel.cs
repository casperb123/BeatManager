using BeatManager.Entities.ModelSaber;
using BeatManager.UserControls;
using ModelSaber.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace BeatManager.ViewModels
{
    public class SaberModelOnlineUserControlViewModel : INotifyPropertyChanged
    {
        private readonly SaberModelOnlineUserControl userControl;
        private OnlineModels onlineModels;

        private int selectedModelsToDownload;
        private int selectedModelsToDelete;
        private Sort currentSort;
        private SortDirection currentSortDirection;

        public List<Sort> SortTypes { get; private set; }
        public List<SortDirection> SortDirectionTypes { get; private set; }

        public Filter FilterAuthor { get; private set; }
        public Filter FilterName { get; private set; }
        public Filter FilterTag { get; private set; }
        public Filter FilterHash { get; private set; }
        public Filter FilterDiscordId { get; private set; }
        public Filter FilterId { get; private set; }

        public MainWindow MainWindow { get; private set; }
        public bool ModelChanged { get; private set; }
        public bool IsLoaded { get; private set; }

        public Sort CurrentSort
        {
            get { return currentSort; }
            set
            {
                currentSort = value;
                OnPropertyChanged(nameof(CurrentSort));
            }
        }

        public SortDirection CurrentSortDirection
        {
            get { return currentSortDirection; }
            set
            {
                currentSortDirection = value;
                OnPropertyChanged(nameof(CurrentSortDirection));
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

        public SaberModelOnlineUserControlViewModel(MainWindow mainWindow, SaberModelOnlineUserControl userControl)
        {
            MainWindow = mainWindow;
            this.userControl = userControl;
            SelectedModels = new List<OnlineModel>();

            SortTypes = Enum.GetValues(typeof(Sort)).Cast<Sort>().ToList();
            SortDirectionTypes = Enum.GetValues(typeof(SortDirection)).Cast<SortDirection>().ToList();

            FilterAuthor = new Filter("author");
            FilterName = new Filter("name");
            FilterTag = new Filter("tag");
            FilterHash = new Filter("hash");
            FilterDiscordId = new Filter("discordid");
            FilterId = new Filter("id");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
