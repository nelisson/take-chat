using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using TakeChat.Domain;
using TakeChat.Domain.Implementations;

namespace TakeChat.Client.Application
{
    class Program
    {
        static void Main()
        {
            try
            {
                using var client = new TcpClient(ServerParams.SERVER_ADDRESS, ServerParams.SERVER_PORT);
                using var stream = client.GetStream();

                using var consoleOut = new StreamWriter(Console.OpenStandardOutput());
                consoleOut.AutoFlush = true;
                Console.SetOut(consoleOut);

                using var consoleIn = new StreamReader(Console.OpenStandardInput());
                Console.SetIn(consoleIn);

                var takeClient = new TakeClient(stream, consoleIn, consoleOut);

                Task.WaitAll(takeClient.ListenToMessages());
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
