using System;
using System.IO;
using System.Text;
using System.Text.Json;
using TakeChat.Domain.Interfaces;

namespace TakeChat.Domain.Entities
{
    public class RegisteredUser : IRegisteredUser
    {
        public string Username { get; set; }
        public string Channel { get; set; }
        private const int BUFFER_SIZE = 1024;
        private readonly Stream _stream;


        public RegisteredUser(Stream stream)
        {
            _stream = stream;
        }

        public void SendMessage(Message message)
        {
            var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _stream.Write(data, 0, data.Length);
        }

        public Message WaitForMessage()
        {
            var buffer = new byte[BUFFER_SIZE];
            int position = _stream.Read(buffer, 0, buffer.Length);
            var jsonResponse = Encoding.UTF8.GetString(buffer, 0, position);

            return JsonSerializer.Deserialize<Message>(jsonResponse);
        }
    }
}
