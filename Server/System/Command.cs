using Server.Models;
using System;
using System.Linq;
using Server.Extensions;
using System.Net.Sockets;
using System.Net;

namespace Server.System
{
    public static class Command
    {
        public static string ProcessCommand(Socket client, string command, CommandSource source)
        {
            bool _logIt = false;
            string result = "601";

            string[] commands = command.Explode(' ', '"');

            if ((command.Count(x => x == '"') % 2 == 0) && commands.Length >= 2)
            {
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
                    else if (commands[0].Equals("user") && commands[1].Equals("connect"))
                    {
                        if (commands.Length == 5)
                            if (Store.Accesses.Any(x => x.Name.Equals(commands[2])))
                                if (Store.Users.Any(x => x.Username.Equals(commands[3])))
                                {
                                    User user = Store.Users.First(x => x.Username.Equals(commands[3]));
                                    if (user.Accesses.Any(x => x.Access.Name.Equals(commands[2])))
                                    {
                                        UserAccess access = user.Accesses.First(x => x.Access.Name.Equals(commands[2]));

                                        if (access.Password.Decrypt().Equals(commands[4].Decrypt()))
                                            result = $"608;;{GenerateToken(client, user, access.Access)}";
                                        else
                                            result = "607";
                                    }
                                    else
                                        if (user.Pendings.Any(x => x.Access.Name.Equals(commands[2])))
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
            }
            else
                return "602";

            return result;
        }

        public static string GenerateToken(Socket client, User user, Access access)
        {
            return $"{((IPEndPoint)client.RemoteEndPoint).Address.ToString()};;{DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm:ss")};;{user.Username};;{access.Name}".Encrypt();
        }
    }

    public enum CommandSource
    {
        Socket, CommandLine
    }
}
