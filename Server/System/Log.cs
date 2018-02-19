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
        #region Confs and log files.
        public static int LOGLENGTH = 1000;

        private static readonly string LOGPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        private static readonly string LOCALCMDDLOG = Path.Combine(LOGPATH, "localcmd.log");
        private static readonly string SOCKETCMDLOG = Path.Combine(LOGPATH, "socketcmd.log");
        #endregion

        #region Logs lists (one for local commands, one for online commands)
        public static List<string> localcommands = new List<string>();
        public static List<string> socketcommands = new List<string>(); 
        #endregion

        /// <summary>
        /// Loading method
        /// </summary>
        public static void LoadLogs()
        {
            if (!Directory.Exists(LOGPATH))
                Directory.CreateDirectory(LOGPATH);

            if (File.Exists(LOCALCMDDLOG))
                localcommands = File.ReadAllLines(LOCALCMDDLOG).ToList();
            if (File.Exists(SOCKETCMDLOG))
                socketcommands = File.ReadAllLines(SOCKETCMDLOG).ToList();
        }

        /// <summary>
        /// Write log method.
        /// </summary>
        /// <param name="content">The content to write.</param>
        /// <param name="type">Source : Local or Remote.</param>
        public static void WriteLog(this string content, CommandSource type)
        {
            List<string> currentLog = (type == CommandSource.CommandLine ? localcommands : (type == CommandSource.Socket ? socketcommands : null));
            string currentpath = (type == CommandSource.CommandLine ? LOCALCMDDLOG : (type == CommandSource.Socket ? SOCKETCMDLOG : null));

            if (currentLog == null || currentpath == null) return;

            currentLog.Add(content);
            File.AppendAllLines(currentpath, new string[] { content });
            if (currentLog.Count >= LOGLENGTH)
            {
                //Manage the old logs to store the new logs
                FileInfo fi = new FileInfo(currentpath);
                File.Move(fi.FullName, $"{Path.Combine(fi.DirectoryName, fi.Name)}.{fi.Directory.GetFiles(fi.Name + "*").Length}{fi.Extension}");
                currentLog.Clear();
            }
        }
    }
}
