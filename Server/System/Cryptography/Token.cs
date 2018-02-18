using Server.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server.System.Cryptography
{
    public class Token
    {
        public string IP { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public Access Access { get; set; }

        public Token(Socket client, User user, Access access)
        {
            this.IP = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
            this.Date = DateTime.Now.AddDays(1);
            this.User = user;
            this.Access = access;
        }

        public Token(string token)
        {
            string[] decrypted = Tornado.Decrypt(token).Split(new string[] { ";;" }, StringSplitOptions.None);
            this.IP = decrypted[0];
            this.Date = DateTime.ParseExact(decrypted[1], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            this.User = Store.Users.FirstOrDefault(x => x.Username.Equals(decrypted[2]));
            this.Access = Store.Accesses.FirstOrDefault(x => x.Name.Equals(decrypted[3]));
        }

        public string Encrypt()
        {
            return Tornado.Encrypt($"{this.IP};;{this.Date.ToString("dd/MM/yyyy HH:mm:ss")};;{this.User.Username};;{this.Access.Name}");
        }

        public string Check(Socket client)
        {
            string result = null;

            if (!this.IP.Equals(((IPEndPoint)client.RemoteEndPoint).Address.ToString())) result = "801";
            else if (this.Date < DateTime.Now) result = "802";
            else if (this.User == null) result = "603";
            else if (!this.User.Active) result = "612";
            else if (this.Access == null) result = "604";
            else if (!this.User.Accesses.Any(x => x.Access == this.Access)) result = "605";

            return result;
        }
    }
}
