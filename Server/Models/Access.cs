using System;

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
    }
}
