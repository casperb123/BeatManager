using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace BeatManager.Entities
{
    public class NamedPipe<T> : IDisposable
    {
        private string pipeName;
        private NamedPipeServerStream pipeServer;
        private bool disposed;
        private Thread thread;
        private bool started;

        public delegate void Request(T t);
        public event Request OnRequest;

        public enum NameTypes
        {
            BeatSaver
        }

        public NamedPipe(NameTypes pipeType)
        {
            disposed = false;
            started = false;
            pipeName = pipeType.ToString();
            thread = new Thread(Main);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = $"NamePipe: {pipeType} Thread";
            thread.IsBackground = true;
        }

        ~NamedPipe()
        {
            Dispose();
        }

        public static void Send(NameTypes pipeType, T t)
        {
            try
            {
                using (var npc = new NamedPipeClientStream(".", pipeType.ToString(), PipeDirection.Out))
                {
                    var bf = new BinaryFormatter();
                    npc.Connect();
                    bf.Serialize(npc, t);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
        }

        public static T Receive(NameTypes pipeType)
        {
            using (var nps = new NamedPipeServerStream(pipeType.ToString(), PipeDirection.In))
            {
                return Receive(nps);
            }
        }

        private static T Receive(NamedPipeServerStream nps)
        {
            var bf = new BinaryFormatter();

            try
            {
                nps.WaitForConnection();
                var obj = bf.Deserialize(nps);

                if (obj is T t)
                    return t;
            }
            catch (IOException) { }
            return default;
        }

        public void Start()
        {
            if (!disposed && !started)
            {
                started = true;
                thread.Start();
            }
        }

        public void Stop()
        {
            started = false;

            if (pipeServer != null)
                pipeServer.Close();
        }

        private void Main()
        {
            while (started && !disposed)
            {
                try
                {
                    using (pipeServer = new NamedPipeServerStream(pipeName))
                    {
                        T t = Receive(pipeServer);

                        if (OnRequest != null && started)
                            OnRequest(t);
                    }
                }
                catch (ThreadAbortException) { }
                catch (IOException)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public void Dispose()
        {
            disposed = true;
            Stop();

            if (OnRequest != null)
                OnRequest = null;
        }
    }
}
