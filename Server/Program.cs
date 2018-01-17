using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        public static List<Socket> Clients = new List<Socket>();

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

                myNewThread = new Thread(() => Listen(listener));
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
            foreach(Socket client in Clients)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }

        static void Listen(Socket listener)
        {
            bool keepgoing = true;
            while (keepgoing)
            {
                try
                {
                    Socket handler = listener.Accept();
                    Console.WriteLine($"{((IPEndPoint)handler.RemoteEndPoint).Address.ToString()} connecté.");
                    Clients.Add(handler);

                    Thread myNewThread = new Thread(() => SocketHelper.HandleClient(handler));
                    myNewThread.Start();
                }
                catch(Exception ex)
                { keepgoing = false; }
            }
        }
    }
}
