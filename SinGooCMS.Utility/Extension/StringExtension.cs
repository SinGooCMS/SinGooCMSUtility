using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtension
    {
        #region 转化为大写的人民币

        /// <summary>
        /// 转化为大写的人民币
        /// </summary>
        /// <param name="decMoney"></param>
        /// <returns></returns>
        public static string ToRMB(this decimal decMoney)
        {
            string[] strN = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string[] strC = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[] strA = { "", "圆", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "万亿", "拾", "佰", "仟", "亿亿", };
            int[] nLoc = { 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };

            string strFrom = "";
            string strTo = "";
            string strChar;
            int m, mLast = -1, nCount = 0;

            if (strFrom.Length > strA.GetUpperBound(0) - 1) return "***拜托，这么多钱还需要数吗***";

            if (decMoney < 0)
            {
                decMoney *= -1;
                strTo = "负";
            }

            Int64 n1 = (Int64)decMoney;                   // 元
            strFrom = n1.ToString();

            for (int i = strFrom.Length; i > 0; i--)
            {
                strChar = strFrom.Substring(strFrom.Length - i, 1);
                m = Convert.ToInt32(strChar);
                if (m == 0)
                {
                    // 连续为０时需要补齐中间单位,且只补第一次
                    if (nLoc[i] > 0 && nCount == 0 && strFrom.Length > 1)
                    {
                        strTo = strTo + strA[i];
                        nCount++;
                    }
                }
                else
                {
                    // 补０
                    if (mLast == 0)
                    {
                        strTo = strTo + strC[0];
                    }

                    // 数字转换为大写
                    strTo = strTo + strC[m];
                    // 补足单位
                    strTo = strTo + strA[i];
                    nCount = 0;
                }
                mLast = m;
            }

            Int64 n2 = ((Int64)(decMoney * 100)) % 100;   // 角分
            Int64 n3 = n2 / 10;                     // 角
            Int64 n4 = n2 % 10;                     // 分
            string s2 = "";

            if (n4 > 0)
            {
                s2 = strC[n4] + "分";
                if (n3 > 0)
                {
                    s2 = strC[n3] + "角" + s2;
                }
            }
            else
            {
                if (n3 > 0)
                {
                    s2 = strC[n3] + "角";
                }
            }
            strTo = strTo + s2;

            if (strTo == "") strTo = strC[0];                   // 全0显示为零
            else if (s2 == "") strTo = strTo + "整";            // 无角分显示整
            return strTo;
        }
        #endregion        

        #region 去除空格

        /// <summary>
        /// 去除空格
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string RemoveWhiteSpace(this string strInput)
        {
            bool changed = false;
            char[] output = strInput.ToCharArray();
            int cursor = 0;
            for (int i = 0, size = output.Length; i < size; i++)
            {
                char c = output[i];
                if (Char.IsWhiteSpace(c))
                {
                    changed = true;
                    continue;
                }

                output[cursor] = c;
                cursor++;
            }

            return changed ? new string(output, 0, cursor) : strInput;
        }
        #endregion

        #region HTML代码与TXT格式转换

        /// <summary>
        /// 清除html脚本
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string RemoveHtml(this string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");

            return Htmlstring.Trim();
        }

        /// <summary>
        /// 把HTML代码转换成TXT格式
        /// </summary>
        /// <param name="htmlBlock">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        public static string ToTxt(this string htmlBlock)
        {
            StringBuilder builder = new StringBuilder(htmlBlock);
            builder.Replace("&nbsp;", " ");
            builder.Replace("<br>", "\r\n");
            builder.Replace("<br>", "\n");
            builder.Replace("<br />", "\n");
            builder.Replace("<br />", "\r\n");
            builder.Replace("&lt;", "<");
            builder.Replace("&gt;", ">");
            builder.Replace("&amp;", "&");
            return builder.ToString();
        }

        /// <summary>
        /// 把TXT代码转换成HTML格式
        /// </summary>
        /// <param name="strTxt">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        public static String ToHtml(this string strTxt)
        {
            StringBuilder sb = new StringBuilder(strTxt);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br />");
            sb.Replace("\n", "<br />");
            sb.Replace("\t", " ");
            return sb.ToString();
        }

        #endregion

        #region url及参数处理

        /// <summary>
        /// url编码
        /// </summary>
        /// <param name="urlString"></param>
        /// <returns></returns>
        public static string AsEncodeUrl(this string urlString) =>
            Uri.EscapeDataString(urlString);

        /// <summary>
        /// url解码
        /// </summary>
        /// <param name="urlString"></param>
        /// <returns></returns>
        public static string AsDecodeUrl(this string urlString) =>
            Uri.UnescapeDataString(urlString);

        /// <summary>
        /// 字典转url查询部分
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToUrlSearch(this IDictionary<string, string> data)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in data)
                builder.Append(Uri.EscapeDataString(item.Key) + "=" + Uri.EscapeDataString(item.Value) + "&");

            return builder.ToString().TrimEnd('&');
        }

        /// <summary>
        /// url查询转字典
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ToUrlDictionary(this string data)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            string[] arrData = Uri.UnescapeDataString(data).Split('&');
            foreach (var item in arrData)
            {
                if (item.IndexOf("=") != -1)
                    result.Add(item.Split('=')[0], item.Split('=')[1]);
            }

            return result;
        }

        #endregion

        #region 掩码

        /// <summary>
        /// 字符串掩码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="mask">掩码符</param>
        /// <returns></returns>
        public static string Mask(this string str, char mask = '*')
        {
            str = str.Trim();
            string masks = mask.ToString().PadLeft(4, mask);
            if (str.Length >= 11)
                return Regex.Replace(str, "(.{3}).*(.{4})", $"$1{masks}$2");
            else if (str.Length == 10)
                return Regex.Replace(str, "(.{3}).*(.{3})", $"$1{masks}$2");
            else if (str.Length == 9)
                return Regex.Replace(str, "(.{2}).*(.{3})", $"$1{masks}$2");
            else if (str.Length == 8)
                return Regex.Replace(str, "(.{2}).*(.{2})", $"$1{masks}$2");
            else if (str.Length == 7)
                return Regex.Replace(str, "(.{1}).*(.{2})", $"$1{masks}$2");
            else if (str.Length == 6)
                return Regex.Replace(str, "(.{1}).*(.{1})", $"$1{masks}$2");
            else
                return Regex.Replace(str, "(.{1}).*", $"$1{masks}");
        }

        #endregion

    }
}
