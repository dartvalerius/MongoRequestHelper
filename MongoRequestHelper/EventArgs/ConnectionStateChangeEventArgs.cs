namespace MongoRequestHelper.EventArgs
{
    public class ConnectionStateChangeEventArgs : BaseEventArgs
    {
        public bool ConnectionState { get; }

        public ConnectionStateChangeEventArgs(bool connectionState)
        {
            ConnectionState = connectionState;
        }
    }
}