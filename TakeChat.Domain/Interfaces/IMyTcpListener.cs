using System.Net.Sockets;
using System.Threading.Tasks;

namespace TakeChat.Domain.Interfaces
{
    public interface IMyTcpListener
    {
        public Task<TcpClient> AcceptTcpClientAsync();
        public void Start();
        public void Stop();
        public bool IsRunning { get; set; }
    }
}
