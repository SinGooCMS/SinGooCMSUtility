using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 提供一组使用<see cref="Uri"/>的辅助方法。
    /// </summary>
    public static class UriExtension
    {
        private static readonly Regex QueryStringRegex = new Regex(@"[?|&]([%23\w\.-]+)=([^?|^&]+)", RegexOptions.Compiled);

        /// <summary>
        /// 从查询字符串中提取参数和值。
        /// </summary>
        /// <remarks>
        /// 此方法还可以正确<c>URL解码</c>解析的键和值。
        /// </remarks>
        public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(this Uri uri)
        {
            var match = QueryStringRegex.Match(uri.OriginalString);
            while (match.Success)
            {
                yield return new KeyValuePair<string, string>(
                    WebUtility.UrlDecode(match.Groups[1].Value),
                    WebUtility.UrlDecode(match.Groups[2].Value));
                match = match.NextMatch();
            }
        }

        #region 追加参数

        /// <summary>
        /// 添加或附加给定的<paramref name="parameter"/>和<paramref name="value"/>
        /// 到<paramref name="uri"/>的查询字符串。
        /// </summary>
        /// <remarks>
        /// 此方法可以正确地<c>URL-encode</c>为给定的<paramref name="parameter"/>和<paramref name="value"/>。
        /// </remarks>
        public static Uri AddParametersToQueryString(this Uri uri, string parameter, string value)
        {
            var queryToAppend = string.Concat(WebUtility.UrlEncode(parameter), "=", WebUtility.UrlEncode(value));
            return AddOrAppendToQueryString(uri, queryToAppend);
        }

        /// <summary>
        /// 将键和值的给定<paramref name="pairs"/>添加或附加到<paramref name="uri"/>的查询字符串中。
        /// </summary>
        /// <remarks>
        /// 该方法也正确地<c>URL-encode</c>的键和值。
        /// </remarks>
        public static Uri AddParametersToQueryString(this Uri uri, IDictionary<string, string> pairs)
        {
            if (!pairs.Any()) { return uri; }

            var keysAndVals = pairs.Select(kv => string.Concat(WebUtility.UrlEncode(kv.Key), "=", WebUtility.UrlEncode(kv.Value)));

            return AddOrAppendToQueryString(uri, string.Join("&", keysAndVals));
        }

        private static Uri AddOrAppendToQueryString(Uri uri, string query)
        {
            var baseUri = new UriBuilder(uri);

            if (baseUri.Query.Length > 1)
            {
                baseUri.Query = uri.Query.Substring(1) + "&" + query;
            }
            else
            {
                baseUri.Query = query;
            }

            return baseUri.Uri;
        }

        #endregion        
    }
}