using TakeChat.Domain.Entities;

namespace TakeChat.Domain.Interfaces
{
    public interface IRegisteredUser
    {
        string Channel { get; set; }
        string Username { get; set; }
        void SendMessage(Message message);
        Message WaitForMessage();
    }
}