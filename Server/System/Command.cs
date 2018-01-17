using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.System
{
    public static class Command
    {
        public static string ProcessCommand(string command, CommandSource source)
        {
            return "";
        }
    }

    public enum CommandSource
    {
        Socket, CommandLine
    }
}
