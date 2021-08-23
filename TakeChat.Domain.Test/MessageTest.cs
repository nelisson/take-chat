using System;
using TakeChat.Domain.Entities;
using Xunit;

namespace TakeChat.Domain.Test
{
    public class MessageTest
    {
        private const string GENERAL_CHANNEL = "general";
        private const string MAXIMUM_CHAR_SENTENCE = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut imperdiet tempor libero eleifend auctor. Duis malesuada urna sed tincidunt eleifend.";

        [Fact]
        public void Constructor_WithEmptyParameterFrom_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Message("", Faker.Internet.UserName(), GENERAL_CHANNEL, MAXIMUM_CHAR_SENTENCE));
        }

        [Fact]
        public void Constructor_WithEmptyParameterToAndChannel_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Message(Faker.Internet.UserName(), "", "", MAXIMUM_CHAR_SENTENCE));
        }

        [Fact]
        public void Constructor_WithEmptyBody_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Message(Faker.Internet.UserName(), Faker.Internet.UserName(), GENERAL_CHANNEL, ""));
        }

        [Fact]
        public void Constructor_WithBodyExcedingMaxCharsAllowed_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Message(Faker.Internet.UserName(), Faker.Internet.UserName(), GENERAL_CHANNEL, MAXIMUM_CHAR_SENTENCE + "A"));
        }

        [Fact]
        public void Constructor_WithCorrectParams_CanGetSameValues()
        {
            var usernameFrom = Faker.Internet.UserName();
            var usernameTo = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;
            var body = MAXIMUM_CHAR_SENTENCE;
            var createdAt = DateTime.UtcNow;

            var message = new Message(usernameFrom, usernameTo, channel, body, createdAt);

            Assert.Equal(usernameFrom, message.From);
            Assert.Equal(usernameTo, message.To);
            Assert.Equal(channel, message.Channel);
            Assert.Equal(body, message.Body);
            Assert.Equal(createdAt, message.CreatedAt);
        }

        [Fact]
        public void Equals_TwoMessagesWithSamePropertyValues_ReturnsTrue()
        {
            var usernameFrom = Faker.Internet.UserName();
            var usernameTo = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;
            var body = MAXIMUM_CHAR_SENTENCE;
            var createdAt = DateTime.UtcNow;

            var message1 = new Message(usernameFrom, usernameTo, channel, body, createdAt);
            var message2 = new Message(usernameFrom, usernameTo, channel, body, createdAt);

            Assert.Equal(message1, message2);
            Assert.Equal(message1.GetHashCode(), message2.GetHashCode());
        }
    }
}
