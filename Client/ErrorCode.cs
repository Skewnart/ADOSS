using System;

namespace Client
{
    public class ErrorCode
    {
        public string Code { get; set; }
        public string FrenchDesc { get; set; }
        public string EnglishDesc { get; set; }

        public ErrorCode(string str)
        {
            string[] strsplitted = str.Split(new string[] { ";;" }, StringSplitOptions.None);

            this.Code = strsplitted[0];
            this.FrenchDesc = strsplitted[1];
            this.EnglishDesc = strsplitted[2];
        }
    }
}
