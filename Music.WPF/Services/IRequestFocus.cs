using System;

namespace Music.WPF.Services
{
    public interface IRequestFocus
    {
        event EventHandler<FocusRequestedEventArgs> FocusRequested;
    }

    public sealed class FocusRequestedEventArgs : EventArgs
    {
        public string PropertyName { get; private set; }

        public FocusRequestedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
