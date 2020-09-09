using System;
using System.Net.NetworkInformation;

namespace BeatSaberSongManager.ViewModels
{
    public class MainWindowViewModel
    {
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
    }
}
