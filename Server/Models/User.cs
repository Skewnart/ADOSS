using Server.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Models
{
    public class User : ISave
    {
        public string Username { get; set; }
        public bool Active { get; set; } = true;

        public List<UserAccess> Accesses { get; set; } = new List<UserAccess>();
        public List<UserAccess> Pendings { get; set; } = new List<UserAccess>();

        public User()
        {

        }

        public User(string content)
        {
            string[] line = content.Split(new string[] { "||" }, StringSplitOptions.None);
            this.Username = line[0];
            this.Active = line[1] == "1";

            if (!String.IsNullOrEmpty(line[2]))
                this.Accesses = line[2].Split(new string[] { "$$" }, StringSplitOptions.None).Select(x => new UserAccess(x)).ToList();
            if (!String.IsNullOrEmpty(line[3]))
                this.Pendings = line[3].Split(new string[] { "$$" }, StringSplitOptions.None).Select(x => new UserAccess(x)).ToList();
        }

        public string Save()
        {
            return $"{this.Username}||{(this.Active ? 1 : 0)}||{String.Join("$$", this.Accesses)}||{String.Join("$$", this.Pendings)}";
        }

        public override string ToString()
        {
            return $@"{this.Username} ({(this.Active ? Language.DICT[11] : Language.DICT[12] + " ")})
    {Language.DICT[13]} : {String.Join(", ", this.Accesses.Select(x => x.Access.Name).ToArray())}
    {Language.DICT[14]} : {String.Join(", ", this.Pendings.Select(x => x.Access.Name).ToArray())}";
        }
    }
}
