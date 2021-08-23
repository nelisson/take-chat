using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using TakeChat.Domain.Entities;
using TakeChat.Domain.Interfaces;

namespace TakeChat.Domain.Implementations
{
    public class TakeServer : IServer
    {
        private const string GENERAL_CHANNEL = "general";
        private const string SERVER_NAME = "server";
        private bool _isListen = false;
        private readonly TcpListener _listener;
        private readonly TextWriter _streamOut;
        public readonly ConcurrentDictionary<Guid, IRegisteredUser> ConnectedUsers;

        public TakeServer(TcpListener listener, TextWriter streamOut)
        {
            _listener = listener;
            _streamOut = streamOut;
            ConnectedUsers = new ConcurrentDictionary<Guid, IRegisteredUser>();
        }

        public async void ListenToClients()
        {   
            while (_isListen)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(() =>
                {
                    var id = AddClient(client);
                    ListenToMessages(id);
                    client.Close();
                });
            }
        }

        private void ListenToMessages(Guid id)
        {
            var user = ConnectedUsers[id];
            while (true)
            {
                var message = user.WaitForMessage();
                ProcessMessage(message, user, id);
            }
        }

        private void ProcessMessage(Message message, IRegisteredUser user, Guid id)
        {
            if (message.To == SERVER_NAME)
            {
                switch (message.Body)
                {
                    case "CLOSE":
                        user.SendMessage(new Message(SERVER_NAME, message.From, "", "*** Disconnected. Bye!"));                        
                        _streamOut.WriteLine($"User {user.Username} disconnected");
                        ConnectedUsers.Remove(id, out user);
                        Broadcast(new Message(SERVER_NAME, "", user.Channel, $"\"{user.Username}\" has left #{user.Channel}"));
                        throw new TaskCanceledException("Communication closed");
                    case "USERS":
                        user.SendMessage(new Message(SERVER_NAME, message.From, "", $"These are registered users: [{string.Join(", ", ConnectedUsers.Select(c => c.Value.Username))}]"));
                        break;
                }
            }
            else if (string.IsNullOrEmpty(message.To))
            {
                Broadcast(message);
            }
            else
            {
                SendToSpecificUser(message);
            }

        }

        private void SendToSpecificUser(Message message)
        {
            if (string.IsNullOrEmpty(message.Channel))
            {
                SendToSpecificUserPrivately(message);
            }
            else
            {
                Broadcast(message);
            }
        }

        private void SendToSpecificUserPrivately(Message message)
        {
            var userTo = ConnectedUsers.Select(c => c.Value).FirstOrDefault(u => u.Username == message.To);
            userTo.SendMessage(message);
        }

        private void Broadcast(Message message)
        {
            foreach (var user in ConnectedUsers)
            {
                user.Value.SendMessage(message);
            }
        }

        private Guid AddClient(TcpClient client)
        {
            Guid id = Guid.NewGuid();
            var user = new RegisteredUser(client.GetStream());
            if (ConnectedUsers.TryAdd(id, user))
            {
                AskForUsername(id);
            }
            else
            {
                throw new InvalidOperationException("TcpClient cannot be added.");
            }

            return id;
        }

        private async void AskForUsername(Guid id)
        {
            var user = ConnectedUsers[id];
            user.SendMessage(new Message(SERVER_NAME, "me", "", "*** Welcome to our chat server. Please provide a nickname:"));

            var repeatedName = true;
            var userName = "";

            do
            {
                var userNameMessage = user.WaitForMessage();
                userName = userNameMessage.Body;
                repeatedName = ConnectedUsers.Select(c => c.Value).Any(u => u.Username == userName);
                if (repeatedName)
                {
                    user.SendMessage(new Message(SERVER_NAME, "me", "", $"*** Sorry, the nickname {userName} is already taken. Please choose a different one:"));
                }

            } while (repeatedName);

            user.Username = userName;
            user.Channel = GENERAL_CHANNEL;
            ConnectedUsers[id] = user;
            _streamOut.WriteLine($"New user registered as {user.Username} joining {user.Channel}");
            user.SendMessage(new Message(SERVER_NAME, user.Username, "", $"*** You are registered as {user.Username}. Joining #{user.Channel}"));
            await Task.Delay(500);
            user.SendMessage(new Message(SERVER_NAME, user.Username, "", "USER"));
            await Task.Delay(500);
            Broadcast(new Message(SERVER_NAME, "", user.Channel, $"\"{user.Username}\" has joined #{user.Channel}"));
        }

        public void Start()
        {
            try
            {
                _listener.Start();
                _isListen = true;
                _streamOut.WriteLine("Server is running");                
            }
            catch (SocketException)
            {
                throw;
            }
        }

        public void Stop()
        {
            _listener.Stop();
            _isListen = false;
            ConnectedUsers.Clear();
        }
    }
}
