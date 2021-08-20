using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TakeChat.Domain.Entities;
using TakeChat.Domain.EventsArgs;
using Xunit;

namespace TakeChat.Domain.Test
{
    public class MessageReceivedArgsTest
    {
        private const string GENERAL_CHANNEL = "general";
        private const string MESSAGE = "The quick brown fox jumps over the lazy dog";

        [Fact]
        public void Constructor_InvalidJsonString_ThrowsException()
        {
            Assert.Throws<System.Text.Json.JsonException>(() => new MessageReceivedArgs(""));
        }

        [Fact]
        public void Constructor_ValidJsonString_ReturnsMessage()
        {
            var message = new Message(Faker.Internet.UserName(), Faker.Internet.UserName(), GENERAL_CHANNEL, MESSAGE);
            string jsonString = JsonSerializer.Serialize(message);
            var messageReceivedArgs = new MessageReceivedArgs(jsonString);

            Assert.Equal(message, messageReceivedArgs.Message);
        }
    }
}
