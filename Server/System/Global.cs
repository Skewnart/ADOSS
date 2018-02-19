using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Server.System
{
    public static class Global
    {
        public static int PORT = 32000;
        public static int MAXCONNECTIONS = 10;
        private static int MAXSTRLENGTH = 50;

        public static ConsoleColor COLOR1 = ConsoleColor.Red;
        public static ConsoleColor COLOR2 = ConsoleColor.Green;

        private static readonly string CONFIGPATH = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config");

        public static string LineFormat(this string str)
        {
            return str.Length > MAXSTRLENGTH + 3 ? str.Substring(0, MAXSTRLENGTH) + "..." : str;
        }

        public static void DisplayTitle()
        {
            Console.Clear();

            Console.ForegroundColor = COLOR1;
            Console.Write(@"  ___ ");
            Console.ForegroundColor = COLOR2;
            Console.WriteLine(@"______ _____ _____ _____ ");
            Console.ForegroundColor = COLOR1;
            Console.Write(@" / _ \");
            Console.ForegroundColor = COLOR2;
            Console.WriteLine(@"|  _  \  _  /  ___/  ___|");
            Console.ForegroundColor = COLOR1;
            Console.Write(@"/ /_\ \");
            Console.ForegroundColor = COLOR2;
            Console.WriteLine(@" | | | | | \ `--.\ `--. ");
            Console.ForegroundColor = COLOR1;
            Console.Write(@"|  _  |");
            Console.ForegroundColor = COLOR2;
            Console.WriteLine(@" | | | | | |`--. \`--. \");
            Console.ForegroundColor = COLOR1;
            Console.Write(@"| | | |");
            Console.ForegroundColor = COLOR2;
            Console.WriteLine(@" |/ /\ \_/ /\__/ /\__/ /");
            Console.ForegroundColor = COLOR1;
            Console.Write(@"\_| |_/");
            Console.ForegroundColor = COLOR2;
            Console.WriteLine(@"___/  \___/\____/\____/ ");
            Console.ResetColor();
        }

        public static void LoadConfig()
        {
            if (File.Exists(CONFIGPATH))
            {
                string[] lines = File.ReadAllLines(CONFIGPATH);
                foreach (string line in lines.Where(x => !x.StartsWith("#")))
                {
                    string[] values = line.Split(new string[] { "=" }, StringSplitOptions.None).Select(x => x.Trim()).ToArray();
                    switch (values[0])
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
                        case "loglength":
                            int.TryParse(values[1], out Log.LOGLENGTH);
                            break;
                        case "lang":
                            Language.LANG = values[1];
                            break;
                        case "titlecolor1":
                            try
                            {
                                COLOR1 = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), values[1]);
                            }
                            catch (Exception) { }
                            break;
                        case "titlecolor2":
                            try
                            {
                                COLOR2 = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), values[1]);
                            }
                            catch (Exception) { }
                            break;
                    }
                }
            }
            else
                File.WriteAllLines(CONFIGPATH, new string[] { "#Server language\n#lang = en\n", "#Listening port\n#port = 32000\n", "#Max simultaneous connection\n#maxconn = 10\n", "#Max console string length\n#maxstr = 50\n", "#Log number of lines\n#loglength = 1000\n", "#Primary Title color\n#titlecolor1 = Red\n", "#Secondary Title color\n#titlecolor2 = Green" });
        }
    }
}
