using System.Text.Json;
using TakeChat.Domain.Entities;

namespace TakeChat.Domain.EventsArgs
{
    public class MessageReceivedArgs
    {
        public Message Message{ get; }

        public MessageReceivedArgs(string jsonString) 
        {
            Message = JsonSerializer.Deserialize<Message>(jsonString);
        }
    }
}
