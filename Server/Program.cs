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
            //All server loadings :

            //General configurations ("config" file)
            Global.LoadConfig();
            Global.DisplayTitle();

            Console.WriteLine("\nInitialization : ");

            #region Language
            Console.Write("{0, -30}", $"\tLanguage...");
            Language.LoadConfig();
            Console.WriteLine(Language.DICT[0]);
            #endregion

            #region Logs
            Console.Write("{0, -30}", $"\t{Language.DICT[1]}...");
            Log.LoadLogs();
            Console.WriteLine(Language.DICT[0]);
            #endregion

            #region Encryption
            Console.Write("{0, -30}", $"\t{Language.DICT[2]}...");
            RSA.LoadKeys();
            Console.WriteLine(Language.DICT[0]);
            #endregion

            #region Datas
            Console.Write("{0, -30}", $"\t{Language.DICT[3]}...");
            Store.LoadDatas();
            Console.WriteLine(Language.DICT[0]);
            #endregion

            #region Listening socket
            Console.Write("{0, -30}", $"\t{Language.DICT[4]}...");
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Global.PORT); 
            #endregion

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Thread myNewThread = null;

            try
            {
                // Here we bind
                listener.Bind(localEndPoint);
                listener.Listen(Global.MAXCONNECTIONS);
                Console.WriteLine($"{Language.DICT[0]}\n");

                Console.WriteLine($"{Language.DICT[5]} {Global.PORT}.\n");

                // The listening system works on a different thread from the main thread, not to block the administrator.
                myNewThread = new Thread(() => Network.Listen(listener));
                myNewThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //Here the administrator write commands to interact with the server.
            string command = "";
            do
            {
                Console.Write("# ");
                command = Console.ReadLine();
                //Enter "quit" to end the server, and close listening socket.
                if (!command.Equals("quit") && !String.IsNullOrEmpty(command))
                {
                    //Each entered command is processed here.
                    string result = Command.ProcessCommand(null, command, CommandSource.CommandLine);
                    if (!String.IsNullOrEmpty(result))
                        Console.WriteLine(result);
                }
            } while (!command.Equals("quit"));

            listener.Close();
        }
    }
}
