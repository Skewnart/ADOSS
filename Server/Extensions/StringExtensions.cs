using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Extensions
{
    public static class StringExtensions
    {
        public static string[] Explode(this string content, char separator, char escapeCharacter)
        {
            if (content == null || separator == escapeCharacter) return null;
            
            List<StringBuilder> result = new List<StringBuilder>() { new StringBuilder("") };
            if (content.Equals("")) return result.Select(x => x.ToString()).ToArray();

            bool mustEscape = false;
            foreach(char car in content)
            {
                if (car == separator && !mustEscape)
                    result.Add(new StringBuilder(""));
                else if (car == escapeCharacter)
                    mustEscape = !mustEscape;
                else
                    result.Last().Append(car);
            }

            return result.Select(x => x.ToString()).ToArray();
        }
    }
}
