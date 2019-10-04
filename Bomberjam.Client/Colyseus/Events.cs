using System;

namespace Bomberjam.Client.Colyseus
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public ExceptionEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        public ExceptionEventArgs(string message)
        {
            this.Exception = new Exception(message);
        }
    }

    public class MessageEventArgs<T> : EventArgs
    {
        public T Message { get; }

        public MessageEventArgs(T message)
        {
            this.Message = message;
        }
    }

    public class MessageEventArgs : MessageEventArgs<object>
    {
        public MessageEventArgs(object message) : base(message)
        {
        }
    }

    public class StateChangeEventArgs<T> : EventArgs
    {
        public T State { get; }

        public bool IsFirstState { get; }

        public StateChangeEventArgs(T state, bool isFirstState = false)
        {
            this.State = state;
            this.IsFirstState = isFirstState;
        }
    }
}