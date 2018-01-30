using Server.Models;
using System;
using System.Linq;

namespace Server.System
{
    public static class Command
    {
        public static string ProcessCommand(string command, CommandSource source)
        {
            bool _logIt = false;
            string result = "601";

            if (source == CommandSource.CommandLine)
            {

            }
            else if (source == CommandSource.Socket)
            {
                if (command.Equals("errors list"))
                    result = $"601;;La commande n'existe pas.;;Command does not exist." +
                        $"$$602;;La commande est mal formatée.;;Command is malformed." +
                        $"$$603;;L'utilisateur n'existe pas.;;User does not exist." +
                        $"$$604;;Le service demandé n'existe pas.;;Requested service does not exist." +
                        $"$$605;;L'utilisateur n'a pas accès à ce service.;;Access is denied." +
                        $"$$606;;L'accès est en cours d'acceptation.;;Access is still pending." +
                        $"$$607;;Le mot de passe ne correspond pas.;;Password does not match." +
                        $"$$608;;Accès autorisé.;;Access granted.";
                else if (command.StartsWith("user connect"))
                {
                    //user connect access username password
                    string[] parts = command.Split(new string[] { " " }, StringSplitOptions.None);
                    if (parts.Length == 5)
                        if (Store.Accesses.Any(x => x.Name.Equals(parts[2])))
                            if (Store.Users.Any(x => x.Username.Equals(parts[3])))
                            {
                                User user = Store.Users.First(x => x.Username.Equals(parts[3]));
                                if (user.Accesses.Any(x => x.Access.Name.Equals(parts[2])))
                                {
                                    UserAccess access = user.Accesses.First(x => x.Access.Name.Equals(parts[2]));

                                    if (access.Password.Decrypt().Equals(parts[4].Decrypt()))
                                        result = "608"; // + token here. access granted
                                    else
                                        result = "607";
                                }
                                else
                                    if (user.Pendings.Any(x => x.Access.Name.Equals(parts[2])))
                                    result = "606";
                                else
                                    result = "605";
                            }
                            else
                                result = "603";
                        else
                            result = "604";
                    else
                        result = "602";
                }
            }

            if (_logIt)
                Log.WriteLog(command, source);

            return result;
        }
    }

    public enum CommandSource
    {
        Socket, CommandLine
    }
}
