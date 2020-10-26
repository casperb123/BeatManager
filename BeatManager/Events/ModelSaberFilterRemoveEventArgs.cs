using ModelSaber.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatManager.Events
{
    public class ModelSaberFilterRemoveEventArgs : EventArgs
    {
        public Filter Filter { get; private set; }

        public ModelSaberFilterRemoveEventArgs(Filter filter)
        {
            Filter = filter;
        }
    }
}
