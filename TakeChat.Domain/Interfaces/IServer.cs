namespace TakeChat.Domain.Interfaces
{
    interface IServer
    {
        void Start();
        void Stop();
        void ListenToClients();
    }
}
