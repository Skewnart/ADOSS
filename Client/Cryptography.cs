using System;
using System.Linq;
using System.Text;

namespace Client
{
    public static class Cryptography
    {
        private static string ALPHABET = "abcdefghijklmnopq:rstuvwxyzABCDEFGH*IJKLMNOPQRSTUVWXYZ,()\';=1234567890|$ ";

        public static string Encrypt(this string plaintext)
        {
            char[] arr = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString().ToCharArray();
            Array.Reverse(arr);
            string code = (new string(arr)).Substring(0, 6);
            arr = code.ToArray();
            Array.Reverse(arr);
            string code2 = (new string(arr)).Substring(0, 6);

            return Transform(Transform(Transform($"{code2}ZZZ{Transform(Transform(Transform(plaintext, code, true), code2, true), code, true)}", "619743", true), "164792", true), "986521", true);
        }

        public static string Decrypt(this string encrypted)
        {
            string mid = Transform(Transform(Transform(encrypted, "986521", false), "164792", false), "619743", false);

            string[] splitted = mid.Split(new string[] { "ZZZ" }, StringSplitOptions.None);
            if (splitted.Length == 2)
            {
                string code2 = splitted[0];
                encrypted = splitted[1];
                char[] arr = code2.ToCharArray();
                Array.Reverse(arr);
                string code = new string(arr);

                return Transform(Transform(Transform(encrypted, code, false), code2, false), code, false);
            }

            return "";
        }

        private static string Transform(string plaintext, string offset, bool rightdirection)
        {
            StringBuilder sb = new StringBuilder();

            int index = 0;
            foreach (char car in plaintext)
            {
                if (ALPHABET.Contains(car))
                    sb.Append(ALPHABET[(ALPHABET.IndexOf(car) + 3 * Int32.Parse(offset[index].ToString()) * (rightdirection ? 1 : -1) + ALPHABET.Length) % ALPHABET.Length]);
                else
                    sb.Append(car);
                index = (index + 1) % offset.Length;
            }

            return sb.ToString();
        }
    }
}
