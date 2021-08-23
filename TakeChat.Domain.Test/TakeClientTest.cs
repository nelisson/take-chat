using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using TakeChat.Domain.Entities;
using TakeChat.Domain.Implementations;
using Xunit;

namespace TakeChat.Domain.Test
{
    public class TakeClientTest
    {
        private const string GENERAL_CHANNEL = "general";
        private const string A_MESSAGE = "The quick brown fox jumps over the lazy dog";

        [Fact]
        public void ListenToMessage_StartAndRunReceiveExitCommand_CompleteTask()
        {
            var usernameFrom = "server";
            var usernameTo = "me";
            var channel = GENERAL_CHANNEL;
            var body = "*** Disconnected. Bye!";

            var byeMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonByeMessage = JsonSerializer.Serialize(byeMessage);


            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonByeMessage), false);
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textReaderMoq.Setup(x => x.ReadLine()).Returns("/exit");
            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            var result = takeClient.ListenToMessages();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.Equal(readedData, jsonByeMessage);
        }

        [Fact]
        public void ReadAndProccessInput_ProcessExitCommand_ThrowsException()
        {
            using var memoryStream = new MemoryStream();
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textReaderMoq.Setup(x => x.ReadLine()).Returns("/exit");
            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            Assert.Throws<AggregateException>(() => takeClient.ReadAndProccessInput());
        }

        [Fact]
        public void ReadAndProccessInput_ProcessInvalidCommand_ThrowsException()
        {
            using var memoryStream = new MemoryStream();
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textReaderMoq.Setup(x => x.ReadLine()).Returns("/crazy");
            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            Assert.Throws<ArgumentException>(() => takeClient.ReadAndProccessInput());
        }

        [Fact]
        public void ReadAndProccessInput_ProcessUsersCommand_SendMessage()
        {
            var usernameFrom = "me";
            var usernameTo = "server";
            var channel = GENERAL_CHANNEL;
            var body = "USERS";

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            using var memoryStream = new MemoryStream();
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textReaderMoq.Setup(x => x.ReadLine()).Returns("/users");
            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);


            takeClient.ReadAndProccessInput();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.Equal(jsonUserMessage, readedData);
        }

        [Fact]
        public void ReadAndProccessInput_ProcessPrivateMessageCommand_SendMessage()
        {
            var usernameFrom = "me";
            var usernameTo = "another";
            var channel = "";
            var body = "hello";

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            using var memoryStream = new MemoryStream();
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textReaderMoq.Setup(x => x.ReadLine()).Returns("/p another hello");
            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);


            takeClient.ReadAndProccessInput();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.Equal(jsonUserMessage, readedData);
        }

        [Fact]
        public void ReadAndProccessInput_ProcessChannelMessageToUserCommand_SendMessage()
        {
            var usernameFrom = "me";
            var usernameTo = "another";
            var channel = GENERAL_CHANNEL;
            var body = "hello";

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            using var memoryStream = new MemoryStream();
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textReaderMoq.Setup(x => x.ReadLine()).Returns("/u another hello");
            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);


            takeClient.ReadAndProccessInput();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.Equal(jsonUserMessage, readedData);
        }

        [Fact]
        public void ReadAndProccessInput_ProcessChannelMessageCommand_SendMessage()
        {
            var usernameFrom = "me";
            var usernameTo = "";
            var channel = GENERAL_CHANNEL;
            var body = "hello";

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            using var memoryStream = new MemoryStream();
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textReaderMoq.Setup(x => x.ReadLine()).Returns("hello");
            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            takeClient.ReadAndProccessInput();

            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());


            Assert.Equal(jsonUserMessage, readedData);
        }

        [Fact]
        public void ReadFromTcpAndProcess_ProcessUserNameServerMessage_ChangeUserName()
        {
            var usernameFrom = "server";
            var usernameTo = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;
            var body = "USER";

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            var outLines = new List<string>();
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonUserMessage));
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            takeClient.ReadFromTcpAndProcess();

            Assert.Equal(usernameTo, takeClient.UserName);
        }

        [Fact]
        public void ReadFromTcpAndProcess_ProcessServerMessage_PrintMesssageBody()
        {
            var usernameFrom = "server";
            var usernameTo = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;
            var body = A_MESSAGE;

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            var outLines = new List<string>();
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonUserMessage));
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()))
                         .Callback<string>(s => outLines.Add(s));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            takeClient.ReadFromTcpAndProcess();

            Assert.Single(outLines);
            Assert.Equal(body, outLines[0]);
        }

        [Fact]
        public void ReadFromTcpAndProcess_ProcessServerByeMessage_ThrowsException()
        {
            var usernameFrom = "server";
            var usernameTo = "me";
            var channel = GENERAL_CHANNEL;
            var body = "*** Disconnected. Bye!";

            var byeMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonByeMessage = JsonSerializer.Serialize(byeMessage);

            
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonByeMessage));
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            Assert.Throws<AggregateException>(() => takeClient.ReadFromTcpAndProcess());
        }

        [Fact]
        public void ReadFromTcpAndProcess_ProcessChannelMessage_PrintMesssageBody()
        {
            var usernameFrom = Faker.Internet.UserName();
            var usernameTo = "";
            var channel = GENERAL_CHANNEL;
            var body = A_MESSAGE;

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            var outLines = new List<string>();
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonUserMessage));
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()))
                         .Callback<string>(s => outLines.Add(s));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            takeClient.ReadFromTcpAndProcess();

            Assert.Single(outLines);
            Assert.Equal($"{usernameFrom} says: {body}", outLines[0]);
        }

        [Fact]
        public void ReadFromTcpAndProcess_ProcessChannelUserMessage_PrintMesssageBody()
        {
            var usernameFrom = Faker.Internet.UserName();
            var usernameTo = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;
            var body = A_MESSAGE;

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            var outLines = new List<string>();
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonUserMessage));
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()))
                         .Callback<string>(s => outLines.Add(s));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            takeClient.ReadFromTcpAndProcess();

            Assert.Single(outLines);
            Assert.Equal($"{usernameFrom} says to {usernameTo}: {body}", outLines[0]);
        }

        [Fact]
        public void ReadFromTcpAndProcess_ProcessChannelUserPrivateMessage_PrintMesssageBody()
        {
            var usernameFrom = Faker.Internet.UserName();
            var usernameTo = Faker.Internet.UserName();
            var channel = "";
            var body = A_MESSAGE;

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            var outLines = new List<string>();
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonUserMessage));
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textWriterMoq.Setup(x => x.WriteLine(It.IsAny<string>()))
                         .Callback<string>(s => outLines.Add(s));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            takeClient.ReadFromTcpAndProcess();

            Assert.Single(outLines);
            Assert.Equal($"{usernameFrom} says privately to {usernameTo}: {body}", outLines[0]);
        }
    }
}
