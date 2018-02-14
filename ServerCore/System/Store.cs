using Server.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Server.System.Cryptography;

namespace Server.System
{
    public static class Store
    {
        private static readonly string DATAPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datas");

        private static readonly string USERFILE = Path.Combine(DATAPATH, "users");
        private static readonly string ACCESSFILE = Path.Combine(DATAPATH, "accesses");

        public static List<User> Users = new List<User>();
        public static List<Access> Accesses = new List<Access>();

        public static void LoadDatas()
        {
            if (!Directory.Exists(DATAPATH))
                Directory.CreateDirectory(DATAPATH);

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
    }
}
