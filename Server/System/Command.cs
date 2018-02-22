using Server.Extensions;
using Server.Models;
using Server.System.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Server.System
{
    public static class Command
    {
        /// <summary>
        /// Method to process the command, depending on the source.
        /// </summary>
        /// <param name="client">Client processing the command.</param>
        /// <param name="command">The command</param>
        /// <param name="source">The source, for the log</param>
        /// <returns>Returning error code, with optionally an additional information.</returns>
        public static string ProcessCommand(Socket client, string command, CommandSource source)
        {
            bool _logIt = true;
            string result = "601";

            string[] commands = command.Explode(' ', '"');

            if (source == CommandSource.CommandLine)
            {
                if (command.Equals("help"))
                    return
@"  Server :
    user list
    user add {username}
    user delete {username}
    user active on|off {username}
    
    access list
    access add {accessname}
    access delete {accessname}
    access grant {accessname} {username}
    access revoke {accessname} {username}
    access revokeall {username}

    viewlast server|client [linenumber]
    
    help
    quit

  Client :
    user connect access {username} {password}
    user register access {username} {password}
    user changepassword access {username} {oldpassword} {newpassword}

    get {key} {token}
    set {key} {value} {token}
    del {key} {token}
    delall {token}
    
    errors list";
                else if (command.Equals("quit"))
                    result = "";
                else if (command.Equals("user list"))
                    result = String.Join("\n", Store.Users);
                else if (command.StartsWith("user add"))
                {
                    if (commands.Length != 3) result = ErrorCode.GetDescriptionFromCode(602);
                    else if (Store.Users.Any(x => x.Username.Equals(commands[2]))) result = ErrorCode.GetDescriptionFromCode(701);
                    else
                    {
                        Store.Users.Add(new User() { Username = commands[2] });
                        Store.Users.Save();
                        result = "";
                    }
                }
                else if (command.StartsWith("user delete"))
                {
                    if (commands.Length != 3) result = ErrorCode.GetDescriptionFromCode(602);
                    else if (!Store.Users.Any(x => x.Username.Equals(commands[2]))) result = ErrorCode.GetDescriptionFromCode(603);
                    else
                    {
                        Console.Write($"{Language.DICT[6]} : ");
                        if (Console.ReadLine().ToLower().Equals(Language.DICT[7].ToLower()))
                            Store.RemoveUser(Store.Users.First(x => x.Username.Equals(commands[2])));
                        result = "";
                    }
                }
                else if (command.StartsWith("user active"))
                {
                    if (commands.Length != 4 || (!commands[2].Equals("on") && !commands[2].Equals("off"))) result = ErrorCode.GetDescriptionFromCode(602);
                    else if (!Store.Users.Any(x => x.Username.Equals(commands[3]))) result = ErrorCode.GetDescriptionFromCode(603);
                    else
                    {
                        User user = Store.Users.First(x => x.Username.Equals(commands[3]));
                        if ((user.Active && commands[2].Equals("on")) || (!user.Active && commands[2].Equals("off"))) result = user.Active ? Language.DICT[8] : Language.DICT[9];
                        else
                        {
                            user.Active = !user.Active;
                            Store.Users.Save();

                            result = "";
                        }
                    }
                }
                else if (command.Equals("access list"))
                    result = String.Join("\n", Store.Accesses);
                else if (command.StartsWith("access add"))
                {
                    if (commands.Length != 3) result = ErrorCode.GetDescriptionFromCode(602);
                    else if (Store.Accesses.Any(x => x.Name.Equals(commands[2]))) result = ErrorCode.GetDescriptionFromCode(702);
                    else
                    {
                        Store.Accesses.Add(new Access(commands[2]));
                        Store.Accesses.Save();
                        result = "";
                    }
                }
                else if (command.StartsWith("access delete"))
                {
                    if (commands.Length != 3) result = ErrorCode.GetDescriptionFromCode(602);
                    else if (!Store.Accesses.Any(x => x.Name.Equals(commands[2]))) result = ErrorCode.GetDescriptionFromCode(703);
                    else
                    {
                        Console.Write($"{Language.DICT[6]} : ");
                        if (Console.ReadLine().ToLower().Equals(Language.DICT[7].ToLower()))
                        {
                            Store.RemoveAccess(Store.Accesses.First(x => x.Name.Equals(commands[2])));
                        }
                        result = "";
                    }
                }
                else if (command.StartsWith("access grant"))
                {
                    if (commands.Length != 4) result = ErrorCode.GetDescriptionFromCode(602);
                    else if (!Store.Users.Any(x => x.Username.Equals(commands[3]))) result = ErrorCode.GetDescriptionFromCode(603);
                    else if (!Store.Accesses.Any(x => x.Name.Equals(commands[2]))) result = ErrorCode.GetDescriptionFromCode(703);
                    else
                    {
                        User user = Store.Users.First(x => x.Username.Equals(commands[3]));
                        Access access = Store.Accesses.First(x => x.Name.Equals(commands[2]));
                        if (user.Accesses.Any(x => x.Access == access)) result = ErrorCode.GetDescriptionFromCode(706);
                        else
                        {
                            if (user.Pendings.Any(x => x.Access == access))
                            {
                                UserAccess useraccess = user.Pendings.First(x => x.Access == access);
                                user.Accesses.Add(useraccess);
                                user.Pendings.Remove(useraccess);
                            }
                            else
                            {
                                Console.Write($"{Language.DICT[10]} : ");
                                string pass = "";
                                ConsoleKeyInfo key = default(ConsoleKeyInfo);
                                do
                                {
                                    key = Console.ReadKey();
                                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.D3)
                                    {
                                        pass += key.KeyChar;
                                        Console.Write("\b*");
                                    }
                                    else
                                    {
                                        if (key.Key == ConsoleKey.Backspace)
                                        {
                                            Console.Write(" ");
                                            if (pass.Length > 0)
                                            {
                                                pass = pass.Substring(0, (pass.Length - 1));
                                                Console.Write("\b");
                                            }
                                        }
                                    }
                                } while (key.Key != ConsoleKey.Enter);

                                Console.WriteLine();
                                user.Accesses.Add(new UserAccess(access, Tornado.Encrypt(pass)));
                            }

                            Store.Users.Save();
                            result = "";
                        }
                    }
                }
                else if (command.StartsWith("access revoke "))
                {
                    if (commands.Length != 4) result = ErrorCode.GetDescriptionFromCode(602);
                    else if (!Store.Users.Any(x => x.Username.Equals(commands[3]))) result = ErrorCode.GetDescriptionFromCode(603);
                    else if (!Store.Accesses.Any(x => x.Name.Equals(commands[2]))) result = ErrorCode.GetDescriptionFromCode(703);
                    else
                    {
                        User user = Store.Users.First(x => x.Username.Equals(commands[3]));
                        Access access = Store.Accesses.First(x => x.Name.Equals(commands[2]));
                        if (!user.Accesses.Any(x => x.Access == access) && !user.Pendings.Any(x => x.Access == access)) result = ErrorCode.GetDescriptionFromCode(704);
                        else
                        {
                            Console.Write($"{Language.DICT[6]} : ");
                            if (Console.ReadLine().ToLower().Equals(Language.DICT[7].ToLower()))
                            {
                                if (user.Accesses.Any(x => x.Access == access))
                                    user.Accesses.Remove(user.Accesses.First(x => x.Access == access));
                                else if (user.Pendings.Any(x => x.Access == access))
                                    user.Pendings.Remove(user.Pendings.First(x => x.Access == access));

                                Store.Users.Save();
                                result = "";
                            }
                        }
                    }
                }
                else if (command.StartsWith("access revokeall"))
                {
                    if (commands.Length != 3) result = ErrorCode.GetDescriptionFromCode(602);
                    else if (!Store.Users.Any(x => x.Username.Equals(commands[2]))) result = ErrorCode.GetDescriptionFromCode(603);
                    else
                    {
                        User user = Store.Users.First(x => x.Username.Equals(commands[2]));
                        if (user.Accesses.Count == 0 && user.Pendings.Count == 0) result = ErrorCode.GetDescriptionFromCode(705);
                        else
                        {
                            Console.Write($"{Language.DICT[6]} : ");
                            if (Console.ReadLine().ToLower().Equals(Language.DICT[7].ToLower()))
                            {
                                user.Accesses.Clear();
                                user.Pendings.Clear();

                                Store.Users.Save();
                                result = "";
                            }
                        }
                    }
                }
                else if (command.StartsWith("viewlast"))
                {
                    int res = 10;
                    bool isNumber = true;
                    if (commands.Length == 3)
                        isNumber = int.TryParse(commands[2], out res);

                    if (commands.Length != 2 && commands.Length != 3) result = ErrorCode.GetDescriptionFromCode(602);
                    else if (!commands[1].Equals("server") && !commands[1].Equals("client")) result = ErrorCode.GetDescriptionFromCode(707);
                    else if (!isNumber) result = ErrorCode.GetDescriptionFromCode(708);
                    else if (res < 1) result = ErrorCode.GetDescriptionFromCode(709);
                    else
                    {
                        List<string> logs = null;
                        if (commands[1].Equals("server"))
                            logs = Log.localcommands.Skip(Math.Max(0, Log.localcommands.Count - res)).ToList();
                        else if (commands[1].Equals("client"))
                            logs = Log.socketcommands.Skip(Math.Max(0, Log.socketcommands.Count - res)).ToList();
                        logs.Reverse();

                        Console.WriteLine(String.Join("\n", logs));
                        result = "";
                    }
                }
                else
                {
                    _logIt = false;
                    result = ErrorCode.GetDescriptionFromCode(601);
                }
            }
            else if (source == CommandSource.Socket)
            {
                if (command.Equals("errors list"))
                    result = String.Join("$$", ErrorCode.AllCodes.Where(x => x.IsOnline));

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

                            if (Tornado.Decrypt(access.Password).Equals(commands[4]))
                                result = $"608;;{new Token(client, user, access.Access).Encrypt()}";
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
                            user.Pendings.Add(new UserAccess(Store.Accesses.FirstOrDefault(x => x.Name.Equals(commands[2])), Tornado.Encrypt(commands[4])));
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
                            if (!Tornado.Decrypt(access.Password).Equals(commands[4])) result = "607";
                            else if (Tornado.Decrypt(commands[4]).Equals(commands[5])) result = "610";
                            else
                            {
                                access.Password = Tornado.Encrypt(commands[5]);
                                result = "611";
                                Store.Users.Save();
                            }
                        }
                    }
                }
                else if (commands[0].Equals("get"))
                {
                    if (commands.Length != 3) result = "602";
                    else
                    {
                        string key = commands[1];
                        Token token = new Token(commands[2]);

                        string res = token.Check(client);
                        if (res == null)
                        {
                            string value = Store.GetKey(token.User, token.Access, key);
                            result = $"{(String.IsNullOrEmpty(value) ? "803" : $"804;;{value}")}";
                        }
                    }
                }
                else if (commands[0].Equals("set"))
                {
                    if (commands.Length != 4) result = "602";
                    else
                    {
                        string key = commands[1];
                        string value = commands[2];
                        Token token = new Token(commands[3]);

                        string res = token.Check(client);
                        if (res == null)
                            result = Store.SetKey(token.User, token.Access, key, value) ? "805" : "806";
                    }
                }
                else if (commands[0].Equals("del"))
                {
                    if (commands.Length != 3) result = "602";
                    else
                    {
                        string key = commands[1];
                        Token token = new Token(commands[2]);

                        string res = token.Check(client);
                        if (res == null)
                            result = Store.DeleteKey(token.User, token.Access, key) ? "807" : "808";
                    }
                }
                else if (commands[0].Equals("delall"))
                {
                    if (commands.Length != 2) result = "602";
                    else
                    {
                        Token token = new Token(commands[1]);

                        string res = token.Check(client);
                        if (res == null)
                            result = Store.DeleteAllKeys(token.User, token.Access) ? "809" : "810";
                    }
                }
                else
                {
                    _logIt = false;
                    result = "601";
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
