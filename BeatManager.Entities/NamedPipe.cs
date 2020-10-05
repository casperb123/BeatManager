using System;
using System.Collections.Generic;
using System.IO.Pipes;
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

        public enum NameTypes
        {
            BeatSaver
        }

        public NamedPipe(NameTypes pipeType)
        {
            disposed = false;
        }
    }
}
