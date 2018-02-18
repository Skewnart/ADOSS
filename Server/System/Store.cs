using Server.Models;
using Server.System.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.System
{
    public static class Store
    {
        private static readonly string DATAPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datas");
        private static readonly string STOREPATH = Path.Combine(DATAPATH, "store");

        private static readonly string USERFILE = Path.Combine(DATAPATH, "users");
        private static readonly string ACCESSFILE = Path.Combine(DATAPATH, "accesses");

        public static List<User> Users = new List<User>();
        public static List<Access> Accesses = new List<Access>();

        public static void LoadDatas()
        {
            if (!Directory.Exists(DATAPATH))
                Directory.CreateDirectory(DATAPATH);
            if (!Directory.Exists(STOREPATH))
                Directory.CreateDirectory(STOREPATH);

            if (File.Exists(ACCESSFILE))
            {
                Accesses = File.ReadAllLines(ACCESSFILE).Select(x => new Access(Tornado.Decrypt(x))).ToList();
            }
            if (File.Exists(USERFILE))
            {
                Users = File.ReadAllLines(USERFILE).Select(x => new User(Tornado.Decrypt(x))).ToList();
            }
        }

        public static void Save<T>(this List<T> list) where T : ISave
        {
            string file = (typeof(T) == typeof(User) ? USERFILE : (typeof(T) == typeof(Access) ? ACCESSFILE : null));
            if (!String.IsNullOrEmpty(file))
                File.WriteAllLines(file, list.Select(x => Tornado.Encrypt(x.Save())).ToArray());
        }

        public static void RemoveUser(User user)
        {
            Store.Users.Remove(user);
            Store.Users.Save();

            foreach (Access access in Store.Accesses)
            {
                string _accessPath = Path.Combine(STOREPATH, access.Name);
                if (Directory.Exists(_accessPath))
                {
                    string _userpath = Path.Combine(_accessPath, user.Username);
                    if (Directory.Exists(_userpath))
                        Directory.Delete(_userpath, true);
                }
            }
        }

        public static void RemoveAccess(Access access)
        {
            Store.Accesses.Remove(access);
            Store.Accesses.Save();

            foreach (User user in Store.Users)
                user.Accesses.RemoveAll(x => x.Access == access);
            Store.Users.Save();

            string _accessPath = Path.Combine(STOREPATH, access.Name);
            if (Directory.Exists(_accessPath))
                Directory.Delete(_accessPath, true);
        }

        public static bool KeyExists(User user, Access access, string key)
        {
            string _accessStorePath = Path.Combine(STOREPATH, access.Name);
            if (Directory.Exists(_accessStorePath))
            {
                string _keyPath = Path.Combine(_accessStorePath, user.Username);
                if (Directory.Exists(_keyPath))
                {
                    string _keyFile = Path.Combine(_keyPath, key);
                    if (File.Exists(_keyFile))
                        return true;
                }
            }

            return false;
        }

        public static string GetKey(User user, Access access, string key)
        {
            string _accessStorePath = Path.Combine(STOREPATH, access.Name);
            if (Directory.Exists(_accessStorePath))
            {
                string _keyPath = Path.Combine(_accessStorePath, user.Username);
                if (Directory.Exists(_keyPath))
                {
                    string _keyFile = Path.Combine(_keyPath, key);
                    if (File.Exists(_keyFile))
                        return Convert.ToBase64String(File.ReadAllBytes(_keyFile));
                }
            }

            return String.Empty;
        }

        public static bool SetKey(User user, Access access, string key, string value)
        {
            try
            {
                string _accessStorePath = Path.Combine(STOREPATH, access.Name);
                if (!Directory.Exists(_accessStorePath)) Directory.CreateDirectory(_accessStorePath);

                string _keyPath = Path.Combine(_accessStorePath, user.Username);
                if (!Directory.Exists(_keyPath)) Directory.CreateDirectory(_keyPath);

                string _keyFile = Path.Combine(_keyPath, key);
                File.WriteAllBytes(_keyFile, Convert.FromBase64String(value));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteKey(User user, Access access, string key)
        {
            string _accessStorePath = Path.Combine(STOREPATH, access.Name);
            if (Directory.Exists(_accessStorePath))
            {
                string _keyPath = Path.Combine(_accessStorePath, user.Username);
                if (Directory.Exists(_keyPath))
                {
                    string _keyFile = Path.Combine(_keyPath, key);
                    if (File.Exists(_keyFile))
                    {
                        try
                        {
                            File.Delete(_keyFile);
                            return true;
                        }
                        catch(Exception)
                        { }
                    }
                }
            }

            return false;
        }
    }
}
