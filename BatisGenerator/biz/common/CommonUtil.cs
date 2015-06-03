using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatisGenerator.biz.common
{
    public class CommonUtil
    {
        public static String formateString(string str)
        {
            return getFirstLetterUpperString(str).Replace("_", "");
        }
        public static String getFirstLetterUpperString(string str)
        {
            char[] charArray = str.ToCharArray();
            charArray[0] = charArray[0].ToString().ToUpper().ToCharArray()[0];
            return new String(charArray);
        }
    }
}
