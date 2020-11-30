using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// 是否null或者空串（比较准确）
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object thisValue)
        {
            if (thisValue == null)
                return true;
            else if (thisValue.ToString().Trim().Length == 0)
                return true;

            return false;
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
        public static bool IsBetween(this DateTime thisValue, DateTime begin, DateTime end)
        {
            return thisValue >= begin && thisValue <= end;
        }

        /// <summary>
        /// 是否0
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsZero(this object thisValue)
        {
            return (thisValue == null || thisValue.ToString() == "0");
        }

        /// <summary>
        /// 是否整数
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsInt(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^\d+$");
        }

        /// <summary>
        /// 不是整数
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsNoInt(this object thisValue)
        {
            if (thisValue == null) return true;
            return !Regex.IsMatch(thisValue.ToString(), @"^\d+$");
        }

        /// <summary>
        /// 是否GUID
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsGuid(this object thisValue)
        {
            if (thisValue == null) return false;
            Guid outValue = Guid.Empty;
            return Guid.TryParse(thisValue.ToString(), out outValue);
        }

        /// <summary>
        /// 是否日期
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsDate(this object thisValue)
        {
            if (thisValue == null) return false;
            DateTime outValue = DateTime.MinValue;
            return DateTime.TryParse(thisValue.ToString(), out outValue);
        }

        /// <summary>
        /// 是否邮件地址
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsEmail(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }

        /// <summary>
        /// 是否手机号
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsMobile(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^\d{11}$");
        }

        /// <summary>
        /// 是否电话
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsTelephone(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{8}$");
        }

        /// <summary>
        /// 是否身份证号
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsIDCard(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
        }

        /// <summary>
        /// 是否传真
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsFax(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsMatch(this object thisValue, string pattern)
        {
            if (thisValue == null) return false;
            Regex reg = new Regex(pattern);
            return reg.IsMatch(thisValue.ToString());
        }

        /// <summary>
        /// 是否数组
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsStringArray(this string thisValue)
        {
            return (thisValue + "").IsMatch(@"System\.[a-z,A-Z,0-9]+?\[\]");
        }

        /// <summary>
        /// 是否可列举的
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsEnumerable(this string thisValue)
        {
            return (thisValue + "").StartsWith("System.Linq.Enumerable");
        }

        /// <summary>
        /// 是否英文
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsEn(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^[a-zA-Z]+$");
        }

        /// <summary>
        /// 是否中文
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsZHCN(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 是否URL
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsUrl(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^[a-zA-z]+://(\\w+(-\\w+)*)(\\.(\\w+(-\\w+)*))*(\\?\\S*)?$");
        }

        /// <summary>
        /// 是否IP
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsIP(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 是否小数
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsDecimal(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^[+-]?[0-9]+[.]?[0-9]+$");
        }

        /// <summary>
        /// 根据文件后缀名来判断是否图片文件
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        public static bool IsImage(this string strFileName)
        {
            string[] arrImageExt = { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };
            return arrImageExt.Contains(System.IO.Path.GetExtension(strFileName).ToLower());
        }

        #endregion
    }
}
