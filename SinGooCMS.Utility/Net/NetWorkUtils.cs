using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Linq;
using SinGooCMS.Utility.Extension;
using System.Net.NetworkInformation;
using System.Net.Http;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 模拟 httpget 和 httppost
    /// </summary>
    public sealed class NetWorkUtils
    {
        #region HttpGet

        /// <summary>
        /// 读取get内容
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="encodingType"></param>
        /// <returns></returns>
        public static string HttpGet(string strUrl, int timeOut = 30 * 1000, string encodingType = "utf-8")
        {
            string reqCookie = string.Empty;
            string resCookie = string.Empty;
            return HttpGet(strUrl, timeOut, encodingType, ref reqCookie, ref resCookie);
        }

        /// <summary>
        /// 读取get内容
        /// </summary>
        /// <param name="strUrl">网址</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="encodingType">编码</param>
        /// <param name="reqCookies">请求的cookie</param>
        /// <param name="rspCookies">输出的cookie</param>
        /// <param name="isHttps">是否ssl</param>
        /// <param name="headers">请求头信息</param>
        /// <returns></returns>
        public static string HttpGet(string strUrl, int timeOut, string encodingType, ref string reqCookies,
            ref string rspCookies, bool isHttps = false, IDictionary<string, string> headers = null)
        {
            if (isHttps)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //注意这个要在.net 4.5里才有

            StringBuilder dataReturnString = new StringBuilder();
            Stream dataStream = null;
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            req.Method = "GET";
            req.Timeout = timeOut;
            req.AllowWriteStreamBuffering = true;
            req.AllowAutoRedirect = true;

            WebHeaderCollection heads = new WebHeaderCollection();
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    switch (item.Key.ToLower().Trim())
                    {
                        case "host":
                            req.Host = item.Value;
                            break;
                        case "referer":
                            req.Referer = item.Value;
                            break;
                        case "user-agent":
                            req.UserAgent = item.Value;
                            break;
                        default:
                            heads.Add(item.Key, item.Value);
                            break;
                    }
                }
            }
            req.Headers = heads;

            if (!req.Headers.AllKeys.Contains("Cookie") && !string.IsNullOrEmpty(reqCookies))
                req.Headers.Set("Cookie", reqCookies);

            if (!req.Headers.AllKeys.Contains("User-Agent"))
                req.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.75 Safari/537.36";

            HttpWebResponse rsp = null;

            try
            {
                rsp = (System.Net.HttpWebResponse)req.GetResponse();
                if (rsp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    rspCookies = rsp.Headers.Get("Set-Cookie");
                    dataStream = rsp.GetResponseStream();
                    System.Text.Encoding encode = System.Text.Encoding.GetEncoding(encodingType);
                    StreamReader readStream = new StreamReader(dataStream, encode);
                    char[] cCount = new char[500];
                    int count = readStream.Read(cCount, 0, 256);
                    while (count > 0)
                    {
                        String str = new String(cCount, 0, count);
                        dataReturnString.Append(str);
                        count = readStream.Read(cCount, 0, 256);
                    }

                    return dataReturnString.ToString();
                }

                return string.Empty;
            }
            finally
            {
                if (dataStream != null)
                    dataStream.Close();

                if (rsp != null)
                    rsp.Close();
            }
        }
        #endregion

        #region HttpPost

        /// <summary>
        /// post 提交
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string HttpPost(string strUrl, string postData)
        {
            return HttpPost(strUrl, postData, string.Empty, string.Empty);
        }

        /// <summary>
        /// post 提交
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="postData"></param>
        /// <param name="host"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public static string HttpPost(string strUrl, string postData, string host, string referer)
        {
            string reqCookies = string.Empty;
            string rspCookies = string.Empty;
            IDictionary<string, string> headers = new Dictionary<string, string>();
            if (!host.IsNullOrEmpty())
                headers.Add("Host", host);
            if (!referer.IsNullOrEmpty())
                headers.Add("Referer", referer);

            return HttpPost(strUrl, postData, 30 * 1000, "utf-8", ref reqCookies, ref rspCookies, false, headers);
        }

        /// <summary>
        /// POST请求url,并返回结果
        /// </summary>
        /// <param name="strUrl">网址</param>
        /// <param name="postData">提交的数据</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="encodingType">编码</param>
        /// <param name="reqCookies">请求的cookie</param>
        /// <param name="rspCookies">输出的cookie</param>
        /// <param name="isHttps">是否ssl</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        public static string HttpPost(string strUrl, string postData, int timeOut, string encodingType,
            ref string reqCookies, ref string rspCookies, bool isHttps = false, IDictionary<string, string> headers = null)
        {
            if (isHttps)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = postData.Length; //可能会报错 在写入所有字节之前不能关闭流 实际传输的字节可能大于这些数据长度
            req.Timeout = timeOut;
            req.AllowAutoRedirect = true;

            WebHeaderCollection heads = new WebHeaderCollection();
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    switch (item.Key.ToLower().Trim())
                    {
                        case "host":
                            req.Host = item.Value;
                            break;
                        case "referer":
                            req.Referer = item.Value;
                            break;
                        case "user-agent":
                            req.UserAgent = item.Value;
                            break;
                        default:
                            heads.Add(item.Key, item.Value);
                            break;
                    }
                }
            }
            req.Headers = heads;

            if (!req.Headers.AllKeys.Contains("Cookie") && !string.IsNullOrEmpty(reqCookies))
                req.Headers.Set("Cookie", reqCookies);

            if (!req.Headers.AllKeys.Contains("User-Agent"))
                req.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.75 Safari/537.36";

            StreamWriter swRequestWriter = null;
            HttpWebResponse rsp = null;

            try
            {
                swRequestWriter = new StreamWriter(req.GetRequestStream());
                swRequestWriter.Write(postData);

                if (swRequestWriter != null)
                    swRequestWriter.Close();

                rsp = (HttpWebResponse)req.GetResponse();
                rsp.Headers.Set("Set-Cookie", rspCookies);
                System.Text.Encoding encode = System.Text.Encoding.GetEncoding(encodingType);
                using (StreamReader reader = new StreamReader(rsp.GetResponseStream(), encode))
                {
                    return reader.ReadToEnd();
                }
            }
            finally
            {
                if (swRequestWriter != null)
                    swRequestWriter.Close();

                if (rsp != null)
                    rsp.Close();
            }
        }

        #endregion

        #region 文件下载
        /// <summary>
        /// 下载文件到本地，覆盖现有文件
        /// </summary>
        /// <param name="originalFile"></param>
        /// <param name="targetDir"></param>
        /// <param name="targetFileName"></param>
        /// <returns></returns>
        public static bool Download(string originalFile, string targetDir, string targetFileName = "")
        {
            bool flag = false;
            try
            {
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                //如果没有指定本地文件名，就用原文件名
                if (targetFileName == "")
                {
                    //文件路径如 http://www.abc.com/1.zip
                    targetFileName = originalFile.Substring(originalFile.LastIndexOf("/") + 1);
                }

                string targetFile = Path.Combine(targetDir + "\\" + targetFileName);
                if (File.Exists(targetFile))
                    File.Delete(targetFile); //删除本地已存在的文件

                using (FileStream fs = new FileStream(targetFile, FileMode.Create))
                {
                    //创建请求
                    WebRequest request = WebRequest.Create(originalFile);
                    //接收响应
                    WebResponse response = request.GetResponse();
                    //输出流
                    Stream responseStream = response.GetResponseStream();
                    byte[] bufferBytes = new byte[10000];//缓冲字节数组
                    int bytesRead = -1;
                    while ((bytesRead = responseStream.Read(bufferBytes, 0, bufferBytes.Length)) > 0)
                    {
                        fs.Write(bufferBytes, 0, bytesRead);
                    }
                    if (fs.Length > 0)
                    {
                        flag = true;
                    }
                    //关闭写入
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return flag;
        }
        #endregion

        #region ping工具

        /// <summary>
        /// 是否能ping通IP
        /// </summary>
        /// <param name="hostNameOrAddr">域名或者IP</param>
        /// <returns></returns>
        public static bool Ping(string hostNameOrAddr)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(hostNameOrAddr);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
