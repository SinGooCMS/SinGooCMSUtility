using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.International.Converters.PinYinConverter;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 字符串处理
    /// </summary>
    public static class StringUtils
    {
        #region 获取汉字拼音的第一个字母

        /// <summary>
        /// 获取汉字拼音的首字母
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string GetChineseSpellFirst(string strText)
        {
            var builder = new StringBuilder();
            strText.ToCharArray().ForEach((p) =>
            {
                builder.Append(GetChineseSpell(p.ToString()).Substring(0, 1));
            });

            return builder.ToString();
        }

        /// <summary>
        /// 获取汉字拼音
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string GetChineseSpell(string strText)
        {
            string py = "";
            var builder = new StringBuilder();
            strText.ToCharArray().ForEach((p) =>
            {
                if (!ChineseChar.IsValidChar(p))
                    builder.Append(p.ToString()); //不是有效的汉字，原字输出
                else
                {
                    var chineseChar = new ChineseChar(p);
                    if (chineseChar.PinyinCount > 0)
                    {
                        py = chineseChar.Pinyins[0].ToString();
                        builder.Append(py.Substring(0, py.Length - 1));
                    }
                    else
                        builder.Append(p.ToString());
                }
            });

            return builder.ToString();
        }

        #endregion

        #region 简繁转换

        /// <summary> 
        /// 简体转换为繁体
        /// </summary> 
        /// <param name="str">简体字</param> 
        /// <returns>繁体字</returns> 
        public static string GetTraditional(string str)
        {
            return ChineseConverter.Convert(str, ChineseConversionDirection.SimplifiedToTraditional);//转繁体
        }

        /// <summary> 
        /// 繁体转换为简体
        /// </summary> 
        /// <param name="str">繁体字</param> 
        /// <returns>简体字</returns> 
        public static string GetSimplified(string str)
        {
            return ChineseConverter.Convert(str, ChineseConversionDirection.TraditionalToSimplified);//转简体
        }

        #endregion

        #region 截取字符串

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="len"></param>
        /// <param name="appendString"></param>
        /// <returns></returns>
        public static string Cut(string sourceStr, int len, string appendString = "")
        {
            if (sourceStr.Length >= len)
                return sourceStr.Substring(0, len) + appendString;

            return sourceStr;
        }

        /// <summary>
        /// 截取中间一段字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string Cut(string source, string begin, string end)
        {
            string sub;
            sub = source.Substring(source.IndexOf(begin) + begin.Length);
            sub = sub.Substring(0, sub.IndexOf(end));
            return sub;
        }

        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns></returns>
        public static int GetStringLength(string str) =>
            Encoding.Default.GetBytes(str).Length;

        #endregion

        #region 处理字符        

        /// <summary>
        /// 压缩HTML
        /// </summary>
        /// <returns></returns>
        public static string Compress(string strHTML)
        {
            strHTML = Regex.Replace(strHTML, @">\s+\r", ">");
            strHTML = Regex.Replace(strHTML, @">\s+\n", ">");
            strHTML = Regex.Replace(strHTML, @">\s+<", "><");

            return strHTML;
        }

        /// <summary>
        /// 替换sql语句中的有问题符号
        /// </summary>
        public static string ChkSQL(string str)
        {
            return (str == null) ? "" : str.Replace("'", "''");
        }

        #endregion

        #region 正则分割

        /// <summary>
        /// 在由正则表达式模式定义的位置拆分输入字符串。
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] Split(string pattern, string input)
        {
            Regex regex = new Regex(pattern);
            return regex.Split(input);
        }

        #endregion

        #region 随机数

        /// <summary>
        /// 生成一个新的文件名
        /// </summary>
        /// <returns></returns>
        public static string GetNewFileName() =>
            DateTime.Now.ToString("yyyyMMddffff") + GetRandomString(8);

        /// <summary>
        /// 生成一个全数字随机数(默认10位)，可以用来生成6位验证码
        /// </summary>
        /// <returns></returns>
        public static string GetRandomNumber(int length = 10) =>
            GetRandomString(length, "", true, false, false, false);

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>        
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length = 10, string custom = "", bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpe = false)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            var builder = new StringBuilder();
            if (!custom.IsNullOrEmpty())
                builder.Append(custom);

            if (useNum == true) { builder.Append("0123456789"); }
            if (useLow == true) { builder.Append("abcdefghijklmnopqrstuvwxyz"); }
            if (useUpp == true) { builder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ"); }
            if (useSpe == true) { builder.Append("!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"); }
            string str = builder.ToString();

            var builder2 = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                builder2.Append(str.Substring(r.Next(0, str.Length - 1), 1));
            }
            return builder2.ToString();
        }

        /// <summary>
        /// 生成GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGUID()
        {
            return System.Guid.NewGuid().ToString();
        }
        #endregion

        #region 返回分类前缀

        /// <summary>
        /// 返回空格
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static string GetSpaceChar(int depth)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                builder.Append(System.Web.HttpUtility.HtmlDecode("&nbsp;&nbsp;"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 返回分类前缀
        /// </summary>
        /// <param name="intDept"></param>
        /// <param name="boolIsEnd"></param>
        /// <returns></returns>
        public static string GetCatePrefix(int intDept, bool boolIsEnd)
        {
            char ch = '\x00a0';
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < intDept; i++)
            {
                builder.Append(ch);
            }
            if (boolIsEnd)
                builder.Append("└");
            else
                builder.Append("├");

            return builder.ToString();
        }
        #endregion                

        #region 生成编号
        /// <summary>
        /// 生成编号${year}${month}${day}${hour}${minute}${second}${millisecond}${rnd}
        /// </summary>
        /// <param name="SNFormat"></param>
        /// <returns></returns>
        public static string GenerateSN(string SNFormat)
        {
            return SNFormat.Replace("${year}", System.DateTime.Now.ToString("yyyy")).Replace("${month}", System.DateTime.Now.ToString("MM"))
                .Replace("${day}", System.DateTime.Now.ToString("dd")).Replace("${hour}", System.DateTime.Now.ToString("HH"))
                .Replace("${minute}", System.DateTime.Now.ToString("mm")).Replace("${second}", System.DateTime.Now.ToString("ss"))
                .Replace("${millisecond}", System.DateTime.Now.ToString("ffff")).Replace("${rnd}", StringUtils.GetRandomString(3));
        }
        #endregion

        #region 生成uri地址

        /// <summary>
        /// 生成uri地址
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <param name="protocol">协议 http:// https:// ftp://</param>
        /// <returns></returns>
        public static Uri GenerateUri(string[] parameters, string protocol = "http://")
        {
            if (protocol.IndexOf("://") == -1)
                protocol += "://";

            var builder = new StringBuilder(protocol);
            for (var i = 0; i < parameters.Length; i++)
            {
                if (i == parameters.Length - 1)
                    builder.Append(parameters[i].Replace("//", "/").TrimStart('/').TrimEnd('/'));
                else
                    builder.Append(parameters[i].Replace("//", "/").TrimStart('/').TrimEnd('/') + "/");
            }

            return new Uri(builder.ToString());
        }

        #endregion
    }
}