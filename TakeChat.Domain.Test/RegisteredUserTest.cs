using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TakeChat.Domain.Entities;
using Xunit;

namespace TakeChat.Domain.Test
{
    public class RegisteredUserTest
    {
        private const string GENERAL_CHANNEL = "general";
        private const string A_MESSAGE = "The quick brown fox jumps over the lazy dog";

        [Fact]
        public void Constructor_ValidParameters_ReturnsOK()
        {
            using var memoryStream = new MemoryStream();
            var username = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;

            var registeredUser = new RegisteredUser(memoryStream)
            {
                Username = username,
                Channel = channel
            };


            Assert.Equal(username, registeredUser.Username);
            Assert.Equal(channel, registeredUser.Channel);
        }
        
        [Fact]
        public void SendMessage_WritingToStream_AreCorrectlySended()
        {
            using var memoryStream = new MemoryStream();
            
            var usernameFrom = Faker.Internet.UserName();
            var usernameTo = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;
            var body = A_MESSAGE;

            var message = new Message(usernameFrom, usernameTo, channel, body);
            var jsonMessage = JsonSerializer.Serialize(message);

            var registeredUser = new RegisteredUser(memoryStream);

            registeredUser.SendMessage(message);

            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.Equal(jsonMessage, readedData);
        }

        [Fact]
        public void WaitForMessage_ReadFromStream_AreCorrectlyReceived()
        {
            var usernameFrom = Faker.Internet.UserName();
            var usernameTo = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;
            var body = A_MESSAGE;
            
            var message = new Message(usernameFrom, usernameTo, channel, body);
            var jsonMessage = JsonSerializer.Serialize(message);
            
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));
            var registeredUser = new RegisteredUser(memoryStream);            
            var receivedMessage = registeredUser.WaitForMessage();

            Assert.Equal(message, receivedMessage);
        }
    }
}
