using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.LoadConfig();
            Global.DisplayTitle();

            Console.Write("\nInitialisation... ");
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Global.PORT);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Thread myNewThread = null;

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(Global.MAXCONNECTIONS);

                Console.WriteLine("  terminé.");
                Console.WriteLine($"En écoute sur le port {Global.PORT}.\n");

                myNewThread = new Thread(() => SocketHelper.Listen(listener));
                myNewThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            string command = "";
            while (!(command = Console.ReadLine()).Equals("quit"))
            {

            }
            listener.Close();
            foreach(Socket client in Global.Clients)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }
    }
}
