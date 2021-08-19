using System;
using Xunit;

namespace TakeChat.Domain.Test
{
    public class MessageTest
    {
        private const string GENERAL_CHANNEL = "general";

        [Fact]
        public void Constructor_WithEmptyParameterFrom_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Message("", Faker.Internet.UserName(), GENERAL_CHANNEL, Faker.Lorem.Sentence(10)));
        }

        [Fact]
        public void Constructor_WithEmptyParameterToAndChannel_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Message(Faker.Internet.UserName(), "", "", Faker.Lorem.Sentence(1)));
        }

        [Fact]
        public void Constructor_WithEmptyBody_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Message(Faker.Internet.UserName(), Faker.Internet.UserName(), GENERAL_CHANNEL, ""));
        }

        [Fact]
        public void Constructor_WithBodyExcedingMaxCharsAllowed_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Message(Faker.Internet.UserName(), Faker.Internet.UserName(), GENERAL_CHANNEL, Faker.Lorem.Sentence(100)));
        }

        [Fact]
        public void Constructor_WithCorrectParams_CanGetSameValues()
        {
            var usernameFrom = Faker.Internet.UserName();
            var usernameTo = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;
            var body = Faker.Lorem.Sentence(2);
            
            var message = new Message(usernameFrom, usernameTo, channel, body);

            Assert.Equal(usernameFrom, message.From);
            Assert.Equal(usernameTo, message.To);
            Assert.Equal(channel, message.Channel);
            Assert.Equal(body, message.Body);
        }
    }
}
