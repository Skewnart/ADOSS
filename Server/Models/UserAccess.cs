using Server.System;
using System;

namespace Server.Models
{
    public class UserAccess
    {
        public Access Access { get; set; }
        public string Password { get; set; }

        public UserAccess(Access access, string password)
        {
            this.Access = access;
            this.Password = password;
        }

        public UserAccess(string content)
        {
            string[] _content = content.Split(new string[] { ";;" }, StringSplitOptions.None);

            this.Access = Store.Accesses.Find(x => x.Name.Equals(_content[0]));
            this.Password = _content[1];
        }

        public override string ToString()
        {
            return $"{this.Access.Name};;{this.Password}";
        }
    }
}
