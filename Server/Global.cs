using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace Server
{
    public static class Global
    {
        public static int PORT = 32000;
        public static int MAXCONNECTIONS = 10;
        private static int MAXSTRLENGTH = 50;

        public static List<Socket> Clients = new List<Socket>();

        private static readonly string CONFIGPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");

        public static string LineFormat(this string str)
        {
            return str.Length > MAXSTRLENGTH+3 ? str.Substring(0, MAXSTRLENGTH) + "..." : str;
        }

        public static void DisplayTitle()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"__   __");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@" ______ _____  ___ ______ ___________");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"\ \ / /");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@"| ___ \  ___|/ _ \|  _  \  ___| ___ \");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@" \ V / ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@"| |_/ / |__ / /_\ \ | | | |__ | |_/ /");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@" /   \ ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@"|    /|  __||  _  | | | |  __||    / ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"/ /^\ \");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@"| |\ \| |___| | | | |/ /| |___| |\ \");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"\/   \/");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@"\_| \_\____/\_| |_/___/ \____/\_| \_|");
            Console.ResetColor();
        }

        public static void LoadConfig()
        {
            if (File.Exists(CONFIGPATH))
            {
                string[] lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config"));
                foreach(string line in lines.Where(x => !x.StartsWith("#")))
                {
                    string[] values = line.Split(new string[] { "=" }, StringSplitOptions.None).Select(x => x.Trim()).ToArray();
                    switch(values[0])
                    {
                        case "port":
                            int.TryParse(values[1], out PORT);
                            break;
                        case "maxconn":
                            int.TryParse(values[1], out MAXCONNECTIONS);
                            break;
                        case "maxstr":
                            int.TryParse(values[1], out MAXSTRLENGTH);
                            break;
                    }
                }
            }
            else
                File.WriteAllLines(CONFIGPATH, new string[] { "#port = 32000", "#maxconn = 10", "#maxstr = 50" });
        }
    }
}
