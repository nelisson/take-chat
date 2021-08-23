using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TakeChat.Domain;
using TakeChat.Domain.Implementations;

namespace TakeChat.Server.Application
{
    class Program
    {
        static void Main()
        {
            var listener = new TcpListener(IPAddress.Parse(ServerParams.SERVER_ADDRESS), ServerParams.SERVER_PORT);
            using var consoleOut = new StreamWriter(Console.OpenStandardOutput());
            consoleOut.AutoFlush = true;
            Console.SetOut(consoleOut);

            var server = new TakeServer(new MyTcpListener(listener), consoleOut);

            server.Start();
            Console.ReadLine();
            server.Stop();
        }
    }

}
