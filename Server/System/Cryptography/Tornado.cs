using System;
using System.Linq;
using System.Text;

namespace Server.System.Cryptography
{
    public static class Tornado
    {
        private static string ALPHABET = "abcdefghijklmnopq:rstuvwxyzABCDEFGH*IJKLMNOPQRSTUVWXYZ,()\';=1234567890|$ ";

        public static string Encrypt(string s)
        {
            char[] a = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString().ToCharArray();
            Array.Reverse(a);
            string c = (new string(a)).Substring(0, 6);
            a = c.ToArray();
            Array.Reverse(a);
            string c2 = (new string(a)).Substring(0, 6);

            return T(T(T($"{c2}ZZZ{T(T(T(s, c, true), c2, true), c, true)}", "619743", true), "164792", true), "986521", true);
        }

        public static string Decrypt(string e)
        {
            string m = T(T(T(e, "986521", false), "164792", false), "619743", false);

            string[] s = m.Split(new string[] { "ZZZ" }, StringSplitOptions.None);
            if (s.Length == 2)
            {
                string c2 = s[0];
                e = s[1];
                char[] arr = c2.ToCharArray();
                Array.Reverse(arr);
                string c = new string(arr);

                return T(T(T(e, c, false), c2, false), c, false);
            }

            return "";
        }

        private static string T(string p, string o, bool r)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            foreach (char c in p)
            {
                if (ALPHABET.Contains(c))
                    sb.Append(ALPHABET[(ALPHABET.IndexOf(c) + 3 * Int32.Parse(o[i].ToString()) * (r ? 1 : -1) + ALPHABET.Length) % ALPHABET.Length]);
                else
                    sb.Append(c);
                i = (i + 1) % o.Length;
            }

            return sb.ToString();
        }
    }
}
