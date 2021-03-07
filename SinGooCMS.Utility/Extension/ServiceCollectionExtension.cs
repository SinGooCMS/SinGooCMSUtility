#if NETSTANDARD2_1
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 依赖注入ServiceCollection容器扩展方法
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 注入HttpContext静态对象，方便在任意地方获取HttpContext，services.AddStaticHttpContext();
        /// </summary>
        /// <param name="services"></param>
        public static void AddStaticHttpContext(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// 注入HttpContext静态对象，方便在任意地方获取HttpContext，app.UseStaticHttpContext();
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            UtilsBase.Configure(httpContextAccessor);
            return app;
        }
    }
}
#endif