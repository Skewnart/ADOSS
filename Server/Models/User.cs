using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Models
{
    public class User : ISave
    {
        public string Username { get; set; }

        public List<UserAccess> Accesses { get; set; } = new List<UserAccess>();
        public List<UserAccess> Pendings { get; set; } = new List<UserAccess>();

        public User()
        {

        }

        public User(string content)
        {
            string[] line = content.Split(new string[] { "||" }, StringSplitOptions.None);
            this.Username = line[0];

            if (!String.IsNullOrEmpty(line[1]))
                this.Accesses = line[1].Split(new string[] { "$$" }, StringSplitOptions.None).Select(x => new UserAccess(x)).ToList();
            if (!String.IsNullOrEmpty(line[2]))
                this.Pendings = line[2].Split(new string[] { "$$" }, StringSplitOptions.None).Select(x => new UserAccess(x)).ToList();
        }

        public string Save()
        {
            return $"{this.Username}||{String.Join("$$", this.Accesses)}||{String.Join("$$", this.Pendings)}";
        }

        public override string ToString()
        {
            return $@"{this.Username} :
    Accès : {String.Join(", ", this.Accesses.Select(x => x.Access.Name).ToArray())}
    Validation : {String.Join(", ", this.Pendings.Select(x => x.Access.Name).ToArray())}"; ;
        }
    }
}
