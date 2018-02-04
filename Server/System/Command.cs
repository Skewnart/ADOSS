using Server.Models;
using System;
using System.Linq;
using Server.Extensions;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Server.System
{
    public static class Command
    {
        public static string ProcessCommand(Socket client, string command, CommandSource source)
        {
            bool _logIt = true;
            string result = "601";

            string[] commands = command.Explode(' ', '"');

            if ((command.Count(x => x == '"') % 2 == 0) && commands.Length >= 2)
            {
                if (source == CommandSource.CommandLine)
                {
                    if (command.Equals("help"))
                        result = "";
                    else if (command.Equals("quit"))
                        result = "";
                    else if (command.Equals("user list"))
                        result = String.Join("\n", Store.Users);
                    else if (command.StartsWith("user add"))
                    {
                        if (commands.Length != 3) result = "La commande est mal formatée.";
                        else if (Store.Users.Any(x => x.Username.Equals(commands[2]))) result = "L'utilisateur existe déjà.";
                        else
                        {
                            Store.Users.Add(new User() { Username = commands[2] });
                            Store.Users.Save();
                            result = "";
                        }
                    }
                    else if (command.StartsWith("user delete"))
                    {
                        if (commands.Length != 3) result = "La commande est mal formatée.";
                        else if (!Store.Users.Any(x => x.Username.Equals(commands[2]))) result = "L'utilisateur n'existe pas.";
                        else
                        {
                            Console.Write("Êtes-vous sûr ? (oui pour accepter) : ");
                            if (Console.ReadLine().Equals("oui"))
                            {
                                Store.Users.Remove(Store.Users.First(x => x.Username.Equals(commands[2])));
                                Store.Users.Save();
                            }
                            result = "";
                        }
                    }
                    else
                    {
                        _logIt = false;
                        result = "La commande n'existe pas.";
                    }
                }
                else if (source == CommandSource.Socket)
                {
                    if (command.Equals("errors list"))
                        result = $"601;;La commande n'existe pas.;;Command does not exist." +
                            $"$$602;;La commande est mal formatée.;;Command is malformed." +
                            $"$$603;;L'utilisateur n'existe pas.;;User does not exist." +
                            $"$$604;;Le service demandé n'existe pas.;;Requested service does not exist." +
                            $"$$605;;L'utilisateur n'a pas accès à ce service.;;Access is denied." +
                            $"$$606;;Le service est en cours d'acceptation.;;Service is still pending." +
                            $"$$607;;Le mot de passe n'est pas bon.;;Password incorrect." +
                            $"$$608;;Accès autorisé.;;Access granted." +
                            $"$$609;;L'utilisateur est déjà enregistré.;;User already registered." +
                            $"$$610;;Le mot de passe n'a pas changé.;;Password didn't change." +
                            $"$$611;;Le mot de passe a bien changé.;;Password successfully changed." +
                            $"$$612;;L'utilisateur n'est pas actif.;;User is not active.";
                    else if (commands[0].Equals("user") && commands[1].Equals("connect"))
                    {
                        if (commands.Length != 5) result = "602";
                        else if (Store.Accesses.TrueForAll(x => !x.Name.Equals(commands[2]))) result = "604";
                        else if (Store.Users.TrueForAll(x => !x.Username.Equals(commands[3]))) result = "603";
                        else
                        {
                            User user = Store.Users.First(x => x.Username.Equals(commands[3]));
                            if (!user.Active) result = "612";
                            else if (user.Accesses.Any(x => x.Access.Name.Equals(commands[2])))
                            {
                                UserAccess access = user.Accesses.First(x => x.Access.Name.Equals(commands[2]));

                                if (access.Password.Decrypt().Equals(commands[4].Decrypt()))
                                    result = $"608;;{GenerateToken(client, user, access.Access)}";
                                else
                                    result = "607";
                            }
                            else if (user.Pendings.Any(x => x.Access.Name.Equals(commands[2])))
                                result = "606";
                            else
                                result = "605";
                        }
                    }
                    else if (commands[0].Equals("user") && commands[1].Equals("register"))
                    {
                        if (commands.Length != 5) result = "602";
                        else if (Store.Accesses.TrueForAll(x => !x.Name.Equals(commands[2]))) result = "604";
                        else if (Store.Users.FirstOrDefault(x => x.Username.Equals(commands[3]))?.Accesses.Any(x => x.Access.Name.Equals(commands[2])) ?? false) result = "609";
                        else if (Store.Users.FirstOrDefault(x => x.Username.Equals(commands[3]))?.Pendings.Any(x => x.Access.Name.Equals(commands[2])) ?? false) result = "606";
                        else
                        {
                            if (!Store.Users.Any(x => x.Username.Equals(commands[3]))) Store.Users.Add(new User() { Username = commands[3] });
                            User user = Store.Users.FirstOrDefault(x => x.Username.Equals(commands[3]));
                            if (!user.Active) result = "612";
                            else
                            {
                                user.Pendings.Add(new UserAccess(Store.Accesses.FirstOrDefault(x => x.Name.Equals(commands[2])), commands[4]));
                                result = "606";

                                Store.Users.Save();
                            }
                        }
                    }
                    else if (commands[0].Equals("user") && commands[1].Equals("changepassword"))
                    {
                        if (commands.Length != 6) result = "602";
                        else if (!Store.Accesses.Any(x => x.Name.Equals(commands[2]))) result = "604";
                        else if (!Store.Users.Any(x => x.Username.Equals(commands[3]))) result = "603";
                        else
                        {
                            User user = Store.Users.FirstOrDefault(x => x.Username.Equals(commands[3]));
                            if (!user.Active) result = "612";
                            else if (!user.Accesses.Any(x => x.Access.Name.Equals(commands[2])) && !user.Pendings.Any(x => x.Access.Name.Equals(commands[2]))) result = "605";
                            else
                            {
                                UserAccess access = user.Accesses.Any(x => x.Access.Name.Equals(commands[2])) ? user.Accesses.FirstOrDefault(x => x.Access.Name.Equals(commands[2])) : user.Pendings.FirstOrDefault(x => x.Access.Name.Equals(commands[2]));
                                if (!access.Password.Decrypt().Equals(commands[4].Decrypt())) result = "607";
                                else if (commands[4].Decrypt().Equals(commands[5].Decrypt())) result = "610";
                                else
                                {
                                    access.Password = commands[5];
                                    result = "611";
                                    Store.Users.Save();
                                }
                            }
                        }
                    }
                    else
                        _logIt = false;
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
