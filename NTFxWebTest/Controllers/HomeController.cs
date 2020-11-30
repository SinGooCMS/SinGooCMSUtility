using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;

namespace NTFxWebTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var builder = new StringBuilder("测试如下：<br/>\r\n");

            //WebUtils
            builder.Append($"pid:{WebUtils.GetQueryInt("pid")}<br/>\r\n"); //?pid=1
            //全url
            builder.Append($"全URL：{WebUtils.GetAbsoluteUri()}<br/>\r\n");

            //CacheUtils 缓存
            DateTime dateTime = DateTime.Now;
            var cache = new CacheUtils();

            var cacheDT = DateTime.Now;
            if (cache.ContainKey("time"))
                cacheDT = cache.Get<DateTime>("time");
            else
                cache.Insert<DateTime>("time", dateTime, 3600);

            builder.Append($"当前时间：{dateTime.ToFormatString()} <br/>\r\n");
            builder.Append($"缓存时间：{cacheDT.ToFormatString()} <br/>\r\n");

            //当前网站目录
            builder.Append($"当前网站目录：{SystemUtils.GetMapPath()} <br/>");
            builder.Append($"upload目录：{SystemUtils.GetMapPath("/upload")} <br/>");

            //cookie
            CookieUtils.SetCookie("username", "jsonlee");
            builder.Append($"username cookie: {CookieUtils.GetCookie("username")} <br/>\r\n");

            //session
            SessionUtils.SetSession("username", "刘备");
            builder.Append($"username session: {SessionUtils.GetSession<string>("username")}  <br/>\r\n");

            return Content(builder.ToString());
        }
    }
}