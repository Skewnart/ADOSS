using Server.System;
using Server.System.Cryptography;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.DisplayTitle();
            Console.WriteLine("\nInitialisation : ");

            Console.Write("{0, -30}","\tConfiguration...");
            Global.LoadConfig();
            Console.WriteLine("ok");

            Console.Write("{0, -30}", "\tLogs...");
            Log.LoadLogs();
            Console.WriteLine("ok");
            
            Console.Write("{0, -30}", "\tSystème de chiffrement...");
            RSA.LoadKeys();
            Console.WriteLine("ok");

            Console.Write("{0, -30}", "\tDonnées...");
            Store.LoadDatas();
            Console.WriteLine("ok");
            
            Console.Write("{0, -30}", "\tMise en réseau...");
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Global.PORT);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Thread myNewThread = null;

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(Global.MAXCONNECTIONS);
                Console.WriteLine("ok\n");

                Console.WriteLine($"En écoute sur le port {Global.PORT}.\n");

                myNewThread = new Thread(() => Network.Listen(listener));
                myNewThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            string command = "";
            do
            {
                Console.Write("# ");
                command = Console.ReadLine();
                if (!command.Equals("quit") && !String.IsNullOrEmpty(command))
                {
                    string result = Command.ProcessCommand(null, command, CommandSource.CommandLine);
                    if (!String.IsNullOrEmpty(result))
                        Console.WriteLine(result);
                }
            } while (!command.Equals("quit"));
            
            listener.Close();
        }
    }
}
