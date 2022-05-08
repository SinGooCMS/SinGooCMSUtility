using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// unicode 编解码<br />
    /// "空\u0020格" => "空 格" <br />
    /// "空 格" => "\u7a7a\u20\u683c"
    /// </summary>
    public class UnicodeUtils
    {
        /// <summary>
        /// Unicode编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encode(string str)
        {
            if (str.IsNullOrEmpty()) return str;
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    strResult.Append("\\u");
                    strResult.Append(((int)str[i]).ToString("x4"));
                }
            }
            return strResult.ToString();
        }

        /// <summary>
        /// Unicode解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decode(string str)
        {
            if (str.IsNullOrEmpty()) return str;
            //最直接的方法Regex.Unescape(str);
            Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
            return reg.Replace(str, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        }
    }
}
