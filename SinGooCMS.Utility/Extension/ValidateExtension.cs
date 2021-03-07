using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 验证扩展
    /// </summary>
    public static class ValidateExtension
    {
        #region 判断

        /// <summary>
        /// 是否null或者空字符串
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object thisValue)
        {
            return thisValue != null && thisValue.ToString().Trim().Length > 0
                ? false
                : true;
        }

        /// <summary>
        /// 判断是否json格式字符串
        /// </summary>
        /// <param name="txtJson"></param>
        /// <returns></returns>
        public static bool IsJson(this string txtJson)
        {
            return JsonConvert.DeserializeObject(txtJson) != null;
        }

        /// <summary>
        /// 是否在两者之间（包含）
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsBetween(this int thisValue, int begin, int end)
        {
            return thisValue >= begin && thisValue <= end;
        }
        /// <summary>
        /// 是否在两者之间（包含）
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsBetween(this decimal thisValue, decimal begin, decimal end)
        {
            return thisValue >= begin && thisValue <= end;
        }
        /// <summary>
        /// 是否在两者之间（包含）
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime thisValue, DateTime begin, DateTime end)
        {
            return thisValue >= begin && thisValue <= end;
        }

        /// <summary>
        /// 是否整数
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsInt(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"^\d+$");
        }
        /// <summary>
        /// 不是整数
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsNoInt(this string thisValue)
        {
            return !thisValue.IsInt();
        }

        /// <summary>
        /// 是否GUID
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsGuid(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Guid.TryParse(thisValue, out _);
        }

        /// <summary>
        /// 是否日期
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsDate(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : DateTime.TryParse(thisValue, out _);
        }

        /// <summary>
        /// 是否邮件地址
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsEmail(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }

        /// <summary>
        /// 是否手机号
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsMobile(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"^\d{11}$");
        }

        /// <summary>
        /// 是否电话
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsTelephone(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{8}$");
        }

        /// <summary>
        /// 是否身份证号
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsIDCard(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
        }

        /// <summary>
        /// 是否传真
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsFax(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsMatch(this string thisValue, string pattern)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : new Regex(pattern).IsMatch(thisValue);
        }

        /// <summary>
        /// 是否英文
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsEn(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"^[a-zA-Z]+$");
        }

        /// <summary>
        /// 是否中文
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsZHCN(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 是否URL
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsUrl(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?");
        }

        /// <summary>
        /// 是否IP
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsIP(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 是否小数
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string thisValue)
        {
            return thisValue.IsNullOrEmpty()
                ? false
                : Regex.IsMatch(thisValue, @"^[+-]?[0-9]+[.]?[0-9]+$");
        }

        /// <summary>
        /// 根据文件后缀名来判断是否图片文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsImage(this string fileName)
        {
            return new string[] { ".jpg", ".jpeg", ".gif", ".png", ".bmp" }
            .Contains(System.IO.Path.GetExtension(fileName).ToLower());
        }

        #endregion
    }
}
