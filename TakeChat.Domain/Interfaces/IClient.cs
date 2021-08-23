using System.Threading.Tasks;

namespace TakeChat.Domain.Interfaces
{
    interface IClient
    {
        public Task ListenToMessages();
        public void ReadAndProccessInput();
        public void ReadFromTcpAndProcess();
        public string UserName { get; set; }
    }
}
