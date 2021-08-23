using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TakeChat.Domain.Entities;
using TakeChat.Domain.Interfaces;

namespace TakeChat.Domain.Implementations
{
    public class TakeClient : IClient
    {
        private const int BUFFER_SIZE = 1024;
        private const string GENERAL_CHANNEL = "general";
        private readonly Stream _stream;
        private readonly TextReader _streamIn;
        private readonly TextWriter _streamOut;
        public string UserName { get; set; } = "me";

        public TakeClient(Stream stream, TextReader streamIn, TextWriter streamOut)
        {
            _stream = stream;
            _streamIn = streamIn;
            _streamOut = streamOut;
        }

        public Task ListenToMessages()
        {
            Task readStreamIn = Task.Run(() => ReadFromInputWriteToTcpStream());
            Task writeStreamOut = Task.Run(() => ReadFromTcpStreamWriteToStreamOut());

            return Task.WhenAll(readStreamIn, writeStreamOut);
        }

        private static bool InputTextIsCommand(string inputText)
        {
            return inputText.StartsWith('/');
        }

        private void ReadFromInputWriteToTcpStream()
        {
            while (true)
            {
                try
                {
                    ReadAndProccessInput();
                }
                catch (AggregateException)
                {
                    break;
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }
        }

        public void ReadAndProccessInput()
        {
            var inputText = _streamIn.ReadLine();

            if (InputTextIsCommand(inputText))
            {
                try
                {
                    var message = ProcessInputCommand(inputText);
                    byte[] closeMessage = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(message));
                    if (_stream.CanWrite)
                    {
                        _stream.Write(closeMessage, 0, closeMessage.Length);
                    }

                    if (message.Body == "CLOSE")
                    {
                        throw new AggregateException("Communication closed");
                    }
                }
                catch (ArgumentException)
                {
                    throw;
                }
            }
            else
            {
                var generalMessage = new Message(UserName, "", GENERAL_CHANNEL, inputText);
                byte[] sendData = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(generalMessage));
                if (_stream.CanWrite)
                {
                    _stream.Write(sendData, 0, sendData.Length);
                }
            }
        }

        private Message ProcessInputCommand(string inputText)
        {
            var to = "server";
            var channel = GENERAL_CHANNEL;
            string body;

            switch (inputText)
            {
                case "/exit":
                    body = "CLOSE";
                    break;
                case "/users":
                    body = "USERS";
                    break;
                default:

                    var segments = inputText.Split(' ');
                    if (segments[0] == "/p" || segments[0] == "/u")
                    {
                        to = segments[1];
                        body = string.Join(' ', segments[2..]);

                        if (segments[0] == "/p")
                        {
                            channel = "";
                        }
                    }
                    else
                    {
                        _streamOut.WriteLine("Invalid command");
                        throw new ArgumentException("Invalid input command");
                    }
                    break;
            }

            return new Message(UserName, to, channel, body);
        }

        private void ReadFromTcpStreamWriteToStreamOut()
        {
            while (true)
            {
                try
                {
                    ReadFromTcpAndProcess();
                }
                catch (AggregateException)
                {
                    break;
                }

            }
        }

        public void ReadFromTcpAndProcess()
        {
            int position;
            var buffer = new byte[BUFFER_SIZE];

            while ((position = _stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                var jsonString = Encoding.UTF8.GetString(buffer, 0, position);
                var message = JsonSerializer.Deserialize<Message>(jsonString);
                if (message.From == "server")
                {
                    if (message.Body == "*** Disconnected. Bye!")
                    {
                        throw new AggregateException("Communication closed");
                    }
                    else if (message.Body == "USER")
                    {
                        UserName = message.To;
                    }
                    else
                    {
                        _streamOut.WriteLine(message.Body);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(message.To))
                    {
                        _streamOut.WriteLine($"{message.From} says: {message.Body}");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(message.Channel))
                        {
                            _streamOut.WriteLine($"{message.From} says privately to {message.To}: {message.Body}");
                        }
                        else
                        {
                            _streamOut.WriteLine($"{message.From} says to {message.To}: {message.Body}");
                        }
                    }
                }
            }
        }
    }
}
