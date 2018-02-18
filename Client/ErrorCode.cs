using System;

namespace Client
{
    public class ErrorCode
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public ErrorCode(string str)
        {
            string[] strsplitted = str.Split(new string[] { ";;" }, StringSplitOptions.None);

            this.Code = strsplitted[0];
            this.Description = strsplitted[1];
        }
    }
}
