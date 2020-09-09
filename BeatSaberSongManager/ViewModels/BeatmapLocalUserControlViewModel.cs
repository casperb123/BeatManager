using BeatSaberSongManager.Entities;
using BeatSaberSongManager.UserControls;
using BeatSaverApi;
using BeatSaverApi.Entities;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace BeatSaberSongManager.ViewModels
{
    public class BeatmapLocalUserControlViewModel : INotifyPropertyChanged
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        private LocalBeatmaps localBeatmaps;
        private readonly BeatmapLocalUserControl userControl;
        private readonly BeatSaver beatSaverApi;

        public readonly MainWindow MainWindow;
        public bool SongDeleted;
        public LocalBeatmaps LocalBeatmaps
        {
            get { return localBeatmaps; }
            set
            {
                localBeatmaps = value;
                OnPropertyChanged(nameof(LocalBeatmaps));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public BeatmapLocalUserControlViewModel(BeatmapLocalUserControl userControl, MainWindow mainWindow)
        {
            this.userControl = userControl;
            MainWindow = mainWindow;
            beatSaverApi = new BeatSaver(Settings.CurrentSettings.SongsPath);
        }

        public bool CanConnectToBeatSaver()
        {
            try
            {
                Ping ping = new Ping();
                string host = "https://beatsaver.com";
                byte[] buffer = new byte[32];
                int timeout = 2000;
                PingReply reply = ping.Send(host, timeout, buffer);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void GetBeatmaps(int page = 0)
        {
            MainWindow.progressRingLoading.IsActive = true;
            MainWindow.rectangleLoading.Visibility = Visibility.Visible;
            MainWindow.progressRingLoading.Visibility = Visibility.Visible;

            _ = Task.Run(async () => LocalBeatmaps = await beatSaverApi.GetLocalBeatmaps(Settings.CurrentSettings.SongsPath, page, CanConnectToBeatSaver()));
        }

        public void UpdatePageButtons()
        {
            if (LocalBeatmaps is null)
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
                userControl.buttonLastPage.IsEnabled = false;
                userControl.buttonNextPage.IsEnabled = false;
                return;
            }

            if (LocalBeatmaps != null && LocalBeatmaps.PrevPage.HasValue)
            {
                userControl.buttonFirstPage.IsEnabled = true;
                userControl.buttonPreviousPage.IsEnabled = true;
            }
            else
            {
                userControl.buttonFirstPage.IsEnabled = false;
                userControl.buttonPreviousPage.IsEnabled = false;
            }
            if (LocalBeatmaps != null && LocalBeatmaps.NextPage.HasValue)
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

        public void NextPage()
        {
            GetBeatmaps(LocalBeatmaps.NextPage.Value);
        }

        public void PreviousPage()
        {
            GetBeatmaps(LocalBeatmaps.PrevPage.Value);
        }

        public void FirstPage()
        {
            GetBeatmaps(0);
        }

        public void LastPage()
        {
            GetBeatmaps(LocalBeatmaps.LastPage);
        }

        public void DeleteSong(string key)
        {
            LocalBeatmap song = LocalBeatmaps.Maps.FirstOrDefault(x => x.Key == key);
            beatSaverApi.DeleteSong(song);
            GetBeatmaps(localBeatmaps.CurrentPage);
            SongDeleted = true;
        }
    }
}
