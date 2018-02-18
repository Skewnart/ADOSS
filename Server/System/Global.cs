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

        private static readonly string CONFIGPATH = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config");

        public static string LineFormat(this string str)
        {
            return str.Length > MAXSTRLENGTH+3 ? str.Substring(0, MAXSTRLENGTH) + "..." : str;
        }

        public static void DisplayTitle()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(@"  ___ ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"______ _____ _____ _____ ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(@" / _ \");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"|  _  \  _  /  ___/  ___|");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(@"/ /_\ \");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@" | | | | | \ `--.\ `--. ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(@"|  _  |");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@" | | | | | |`--. \`--. \");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(@"| | | |");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@" |/ /\ \_/ /\__/ /\__/ /");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(@"\_| |_/");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"___/  \___/\____/\____/ ");
            Console.ResetColor();
        }

        public static void LoadConfig()
        {
            if (File.Exists(CONFIGPATH))
            {
                string[] lines = File.ReadAllLines(CONFIGPATH);
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
                        case "loglength":
                            int.TryParse(values[1], out Log.LOGLENGTH);
                            break;
                        case "lang":
                            Language.LANG = values[1];
                            break;
                    }
                }
            }
            else
                File.WriteAllLines(CONFIGPATH, new string[] { "#port = 32000", "#maxconn = 10", "#maxstr = 50", "#loglength = 1000", "#lang = en" });
        }
    }
}
