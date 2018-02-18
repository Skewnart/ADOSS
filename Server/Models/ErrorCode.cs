using Server.System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Models
{
    public class ErrorCode
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public bool IsOnline { get; set; }

        public ErrorCode(int code, string description, bool isonline = true)
        {
            this.Code = code;
            this.Description = description;
            this.IsOnline = isonline;
        }

        public override string ToString()
        {
            return $"{this.Code};;{this.Description}";
        }

        public static readonly HashSet<ErrorCode> AllCodes = new HashSet<ErrorCode>()
        {
            new ErrorCode(601, Language.DICT[15]),
            new ErrorCode(602, Language.DICT[16]),
            new ErrorCode(603, Language.DICT[17]),
            new ErrorCode(604, Language.DICT[18]),
            new ErrorCode(605, Language.DICT[19]),
            new ErrorCode(606, Language.DICT[20]),
            new ErrorCode(607, Language.DICT[21]),
            new ErrorCode(608, Language.DICT[22]),
            new ErrorCode(609, Language.DICT[23]),
            new ErrorCode(610, Language.DICT[24]),
            new ErrorCode(611, Language.DICT[25]),
            new ErrorCode(612, Language.DICT[26]),

            new ErrorCode(701, Language.DICT[27], false),
            new ErrorCode(702, Language.DICT[28], false),
            new ErrorCode(703, Language.DICT[29], false),
            new ErrorCode(704, Language.DICT[30], false),
            new ErrorCode(705, Language.DICT[31], false),
            new ErrorCode(706, Language.DICT[32], false),
            new ErrorCode(707, Language.DICT[33], false),
            new ErrorCode(708, Language.DICT[34], false),
            new ErrorCode(709, Language.DICT[35], false),

            new ErrorCode(801, Language.DICT[36]),
            new ErrorCode(802, Language.DICT[37]),
            new ErrorCode(803, Language.DICT[38]),
            new ErrorCode(804, Language.DICT[39]),
            new ErrorCode(805, Language.DICT[40]),
            new ErrorCode(806, Language.DICT[41]),
            new ErrorCode(807, Language.DICT[42]),
            new ErrorCode(808, Language.DICT[43]),
            new ErrorCode(809, Language.DICT[44]),
            new ErrorCode(810, Language.DICT[45]),
        };

        public static string GetDescriptionFromCode(int code)
        {
            return AllCodes.FirstOrDefault(x => x.Code == code)?.Description;
        }
    }
}
