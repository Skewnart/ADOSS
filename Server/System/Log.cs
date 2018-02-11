using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.System
{
    public static class Log
    {
        public static int LOGLENGTH = 1000;

        private static readonly string LOGPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        private static readonly string LOCALCMDDLOG = Path.Combine(LOGPATH, "localcmd.log");
        private static readonly string SOCKETCMDLOG = Path.Combine(LOGPATH, "socketcmd.log");

        public static List<string> localcommands = new List<string>();
        public static List<string> socketcommands = new List<string>();

        public static void LoadLogs()
        {
            if (!Directory.Exists(LOGPATH))
                Directory.CreateDirectory(LOGPATH);

            if (File.Exists(LOCALCMDDLOG))
                localcommands = File.ReadAllLines(LOCALCMDDLOG).ToList();
            if (File.Exists(SOCKETCMDLOG))
                socketcommands = File.ReadAllLines(SOCKETCMDLOG).ToList();
        }

        public static void WriteLog(this string content, CommandSource type)
        {
            List<string> currentLog = (type == CommandSource.CommandLine ? localcommands : (type == CommandSource.Socket ? socketcommands : null));
            string currentpath = (type == CommandSource.CommandLine ? LOCALCMDDLOG : (type == CommandSource.Socket ? SOCKETCMDLOG : null));

            if (currentLog == null || currentpath == null) return;

            currentLog.Add(content);
            File.AppendAllLines(currentpath, new string[] { content });
            if (currentLog.Count >= LOGLENGTH)
            {
                FileInfo fi = new FileInfo(currentpath);
                File.Move(fi.FullName, $"{Path.Combine(fi.DirectoryName, fi.Name)}.{fi.Directory.GetFiles(fi.Name + "*").Length}{fi.Extension}");
                currentLog.Clear();
            }
        }
    }
}
