using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace BeatSaberSongManager.ViewModels
{
    public class MainWindowViewModel
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public bool IsConnectedToInternet()
        {
            return InternetGetConnectedState(out _, 0);
        }
    }
}
