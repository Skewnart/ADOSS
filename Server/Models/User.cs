using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class User
    {
        public string EMail { get; set; }

        public List<UserAccess> Accesses { get; set; }
        public List<UserAccess> Pendings { get; set; }

        public User(string email, string password)
        {
            this.EMail = email;

            this.Accesses = new List<UserAccess>();
            this.Pendings = new List<UserAccess>();
        }

        public User(string str)
        {

        }
    }
}
