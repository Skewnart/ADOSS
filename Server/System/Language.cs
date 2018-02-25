using System;
using System.IO;

namespace Server.System
{
    public static class Language
    {
        private static readonly string LANGPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lang");
        public static string[] DICT;
        public static string LANG = "en";

        public static void LoadConfig()
        {
            if (!Directory.Exists(LANGPATH))
                Directory.CreateDirectory(LANGPATH);

            string langfile = Path.Combine(LANGPATH, LANG);

            if (!File.Exists(langfile) || (DICT = File.ReadAllLines(langfile)).Length != 46)
            {
                langfile = Path.Combine(LANGPATH, "en");
                if (!File.Exists(langfile) || (DICT = File.ReadAllLines(langfile)).Length != 46)
                    File.WriteAllLines(langfile, new string[]
                        {
                            "ok",
                            "Logs",
                            "Encryption system",
                            "Datas",
                            "Networking",
                            "Listening on port",
                            "Are you sure ?",
                            "Yes",
                            "User is already active",
                            "User is already inactive",
                            "Password",
                            "Active",
                            "Inactive",
                            "Accesses",
                            "Pendings",
                            "Command does not exist.",
                            "Command is malformed.",
                            "User does not exist.",
                            "Requested service does not exist.",
                            "Access is denied.",
                            "Service is still pending.",
                            "Password incorrect.",
                            "Access granted.",
                            "User already registered.",
                            "Password didn't change.",
                            "Password successfully changed.",
                            "User is not active.",
                            "User already exist.",
                            "Service already exist.",
                            "Service does not exist.",
                            "User does not have this service access.",
                            "User does not have any service access.",
                            "User already have this service access.",
                            "Log type must be \"server\" or \"client\".",
                            "Parameter is not a number.",
                            "Given number is not valid.",
                            "You are not the token's owner.",
                            "Token is not valid anymore.",
                            "Data does not exist.",
                            "Data exists.",
                            "Data has been stored.",
                            "Data couldn't be stored.",
                            "Data has been deleted.",
                            "Data couldn't be deleted.",
                            "All datas have been deleted.",
                            "All datas couldn't be deleted."
                        });

                DICT = File.ReadAllLines(langfile);
            }
        }
    }
}
