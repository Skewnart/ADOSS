using Server.System;
using System;
using System.Linq;

namespace Server.Models
{
    public class Access : ISave
    {
        public string Name { get; set; }

        public Access(string name)
        {
            this.Name = name;
        }

        public string Save()
        {
            return this.Name;
        }

        public override string ToString()
        {
            return $"{this.Name} : {String.Join(", ", Store.Users.Where(x => x.Accesses.Any(y => y.Access.Name.Equals(this.Name))).Select(x => x.Username).ToArray())}";
        }
    }
}
