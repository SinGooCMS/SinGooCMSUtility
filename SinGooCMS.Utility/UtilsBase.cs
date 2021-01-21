using System;
#if NETSTANDARD2_1
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 工具底层，主要获取 http上下文 HttpContext。netfx从system.web中读取，core需要注入
    /// </summary>
    internal sealed class UtilsBase
    {
#if NETSTANDARD2_1

        private static IHttpContextAccessor _accessor;

        /// <summary>
        /// 获取当前禽求上下文，使用前需要在Startup中注入，ConfigureServices方法中：services.AddStaticHttpContext();，Configure方法中：app.UseStaticHttpContext();
        /// </summary>
        public static HttpContext HttpContext => _accessor?.HttpContext;

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// 请求
        /// </summary>
        public static HttpRequest Request => HttpContext?.Request;

        /// <summary>
        /// 输出
        /// </summary>
        public static HttpResponse Response => HttpContext?.Response;
#else

        /// <summary>
        /// http上下文
        /// </summary>
        public static HttpContext HttpContext => System.Web.HttpContext.Current;
        /// <summary>
        /// 请求
        /// </summary>
        public static HttpRequest Request => System.Web.HttpContext.Current?.Request;
        /// <summary>
        /// 输出
        /// </summary>
        public static HttpResponse Response => System.Web.HttpContext.Current?.Response;

#endif
    }
}