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
        #region Data files
        private static readonly string DATAPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datas");
        private static readonly string STOREPATH = Path.Combine(DATAPATH, "store");

        private static readonly string USERFILE = Path.Combine(DATAPATH, "users");
        private static readonly string ACCESSFILE = Path.Combine(DATAPATH, "accesses");
        #endregion

        #region Storing lists
        public static List<User> Users = new List<User>();
        public static List<Access> Accesses = new List<Access>(); 
        #endregion

        /// <summary>
        /// Data loading
        /// </summary>
        public static void LoadDatas()
        {
            if (!Directory.Exists(DATAPATH))
                Directory.CreateDirectory(DATAPATH);
            if (!Directory.Exists(STOREPATH))
                Directory.CreateDirectory(STOREPATH);

            if (File.Exists(ACCESSFILE))
                Accesses = File.ReadAllLines(ACCESSFILE).Select(x => new Access(Tornado.Decrypt(x))).ToList();
            if (File.Exists(USERFILE))
                Users = File.ReadAllLines(USERFILE).Select(x => new User(Tornado.Decrypt(x))).ToList();
        }

        /// <summary>
        /// Storing method. Works with users and accesses
        /// </summary>
        /// <typeparam name="T">Can be for users or accesses. More generally, works with classes implementing the ISave interface.</typeparam>
        /// <param name="list">Saving list</param>
        public static void Save<T>(this List<T> list) where T : ISave
        {
            //Datas are stored with the Tornado encryption system.
            string file = (typeof(T) == typeof(User) ? USERFILE : (typeof(T) == typeof(Access) ? ACCESSFILE : null));
            if (!String.IsNullOrEmpty(file))
                File.WriteAllLines(file, list.Select(x => Tornado.Encrypt(x.Save())).ToArray());
        }

        /// <summary>
        /// Remove user method
        /// </summary>
        /// <param name="user">User to be stored</param>
        public static void RemoveUser(User user)
        {
            //Remove from users list, and save.
            Store.Users.Remove(user);
            Store.Users.Save();

            //Remove stored datas.
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

        /// <summary>
        /// Remove access method
        /// </summary>
        /// <param name="access">Access to be stored</param>
        public static void RemoveAccess(Access access)
        {
            //Remove from accesses list, and save.
            Store.Accesses.Remove(access);
            Store.Accesses.Save();

            //Remove access from all users.
            foreach (User user in Store.Users)
                user.Accesses.RemoveAll(x => x.Access == access);
            Store.Users.Save();

            //Remove stored datas.
            string _accessPath = Path.Combine(STOREPATH, access.Name);
            if (Directory.Exists(_accessPath))
                Directory.Delete(_accessPath, true);
        }

        /// <summary>
        /// Check if key for a given user and access exists.
        /// </summary>
        /// <param name="user">User to check</param>
        /// <param name="access">Access to check</param>
        /// <param name="key">Key to check</param>
        /// <returns>True if key exists</returns>
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

        /// <summary>
        /// Get value from key, for a given user and access
        /// </summary>
        /// <param name="user">User to check</param>
        /// <param name="access">Access to check</param>
        /// <param name="key">Value to get for the given key</param>
        /// <returns>The value for the given key. Empty if not found. RESULT in 64digits.</returns>
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

        /// <summary>
        /// Set a value for a key, for a given user and access
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="access">The access</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>True if the value has been set. False otherwise</returns>
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

        /// <summary>
        /// Delete a given key, for a given user and access
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="access">The access</param>
        /// <param name="key">The key to delete</param>
        /// <returns>True if the key has been deleted. False otherwise</returns>
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
                        catch (Exception)
                        { }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Delete all keys for a given user and access
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="access">The access</param>
        /// <returns>True if one or more files have been deleted. False otherwise.</returns>
        public static bool DeleteAllKeys(User user, Access access)
        {
            string _accessStorePath = Path.Combine(STOREPATH, access.Name);
            if (Directory.Exists(_accessStorePath))
            {
                string _keyPath = Path.Combine(_accessStorePath, user.Username);
                if (Directory.Exists(_keyPath))
                {
                    string[] files = Directory.GetFiles(_keyPath);
                    foreach(string file in files)
                    { 
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception)
                        { }
                    }
                    return Directory.GetFiles(_keyPath).Length == 0;
                }
            }

            return true;
        }
    }
}
