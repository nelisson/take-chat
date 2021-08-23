using Moq;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TakeChat.Domain.Entities;
using TakeChat.Domain.Implementations;
using Xunit;

namespace TakeChat.Domain.Test
{
    public class TakeClientTest
    {
        private const string GENERAL_CHANNEL = "general";

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

            textReaderMoq.Setup(x => x.ReadLineAsync()).ReturnsAsync("/exit");
            textWriterMoq.Setup(x => x.WriteLineAsync(It.IsAny<string>()));            

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            var result = takeClient.ListenToMessages();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.True(result.IsCompleted);
            Assert.Equal(readedData, jsonByeMessage);
        }

        [Fact]
        public void ReadAndProccessInput_ProcessExitCommand_ThrowsException()
        {
            using var memoryStream = new MemoryStream();
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textReaderMoq.Setup(x => x.ReadLineAsync()).ReturnsAsync("/exit");
            textWriterMoq.Setup(x => x.WriteLineAsync(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);
            
            Assert.ThrowsAsync<AggregateException>(() => takeClient.ReadAndProccessInput());
        }

        [Fact]
        public void ReadAndProccessInput_ProcessInvalidCommand_ThrowsException()
        {
            using var memoryStream = new MemoryStream();
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textReaderMoq.Setup(x => x.ReadLineAsync()).ReturnsAsync("/crazy");
            textWriterMoq.Setup(x => x.WriteLineAsync(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            Assert.ThrowsAsync<ArgumentException>(() => takeClient.ReadAndProccessInput());
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

            textReaderMoq.Setup(x => x.ReadLineAsync()).ReturnsAsync("/users");
            textWriterMoq.Setup(x => x.WriteLineAsync(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);


            var result = takeClient.ReadAndProccessInput();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.True(result.IsCompleted);
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

            textReaderMoq.Setup(x => x.ReadLineAsync()).ReturnsAsync("/p another hello");
            textWriterMoq.Setup(x => x.WriteLineAsync(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);


            var result = takeClient.ReadAndProccessInput();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.True(result.IsCompleted);
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

            textReaderMoq.Setup(x => x.ReadLineAsync()).ReturnsAsync("/u another hello");
            textWriterMoq.Setup(x => x.WriteLineAsync(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);


            var result = takeClient.ReadAndProccessInput();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.True(result.IsCompleted);
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

            textReaderMoq.Setup(x => x.ReadLineAsync()).ReturnsAsync("hello");
            textWriterMoq.Setup(x => x.WriteLineAsync(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);


            var result = takeClient.ReadAndProccessInput();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            Assert.True(result.IsCompleted);
            Assert.Equal(jsonUserMessage, readedData);
        }

        [Fact]
        public void ReadFromTcpAndProcess_ProcessUserNameServerMessage_ReceiveMessage()
        {
            var usernameFrom = "server";
            var usernameTo = Faker.Internet.UserName();
            var channel = GENERAL_CHANNEL;
            var body = "USER";

            var userMessage = new Message(usernameFrom, usernameTo, channel, body);
            var jsonUserMessage = JsonSerializer.Serialize(userMessage);

            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonUserMessage));
            var textReaderMoq = new Mock<TextReader>();
            var textWriterMoq = new Mock<TextWriter>();

            textWriterMoq.Setup(x => x.WriteLineAsync(It.IsAny<string>()));

            var takeClient = new TakeClient(memoryStream, textReaderMoq.Object, textWriterMoq.Object);

            //TODO: configurar o moq para salvar o que for escrito no writeline
            var result = takeClient.ReadFromTcpAndProcess();
            var readedData = Encoding.UTF8.GetString(memoryStream.ToArray());

            textWriterMoq.Verify();
        }
    }
}
