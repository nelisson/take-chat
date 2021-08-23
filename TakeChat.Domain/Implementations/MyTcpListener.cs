using System.Net.Sockets;
using System.Threading.Tasks;
using TakeChat.Domain.Interfaces;

namespace TakeChat.Domain.Implementations
{
    public class MyTcpListener : IMyTcpListener
    {
        private readonly TcpListener _tcpListener;
        public bool IsRunning { get; set; }

        public MyTcpListener(TcpListener tcpListener)
        {
            _tcpListener = tcpListener;
        }

        public Task<TcpClient> AcceptTcpClientAsync()
        {
            return _tcpListener.AcceptTcpClientAsync();
        }

        public void Start()
        {
            _tcpListener.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            _tcpListener.Stop();
            IsRunning = false;
        }
    }
}
