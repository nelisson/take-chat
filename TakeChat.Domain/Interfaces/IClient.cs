using System.Threading.Tasks;

namespace TakeChat.Domain.Interfaces
{
    interface IClient
    {
        public Task ListenToMessages();
        public Task ReadAndProccessInput();
        public Task ReadFromTcpAndProcess();
    }
}
