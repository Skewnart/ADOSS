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
            Global.LoadConfig();
            Global.DisplayTitle();

            Console.WriteLine("\nInitialization : ");

            Console.Write("{0, -30}", $"\tLanguage...");
            Language.LoadConfig();
            Console.WriteLine(Language.DICT[0]);

            Console.Write("{0, -30}", $"\t{Language.DICT[1]}...");
            Log.LoadLogs();
            Console.WriteLine(Language.DICT[0]);
            
            Console.Write("{0, -30}", $"\t{Language.DICT[2]}...");
            RSA.LoadKeys();
            Console.WriteLine(Language.DICT[0]);

            Console.Write("{0, -30}", $"\t{Language.DICT[3]}...");
            Store.LoadDatas();
            Console.WriteLine(Language.DICT[0]);
            
            Console.Write("{0, -30}", $"\t{Language.DICT[4]}...");
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Global.PORT);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Thread myNewThread = null;

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(Global.MAXCONNECTIONS);
                Console.WriteLine($"{Language.DICT[0]}\n");

                Console.WriteLine($"{Language.DICT[5]} {Global.PORT}.\n");

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
