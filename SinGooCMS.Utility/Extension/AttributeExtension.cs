using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;

#if NETSTANDARD2_1
using Microsoft.AspNetCore.Routing;
#endif

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 特性扩展类<br/>
    /// 属性的显示扩展,优先使用资源Name,默认使用自带的Name,注意：仅适用于MVC
    /// <code>
    /// [DisplayExt(Name = "辅料料号",ResName = "pt_no",ResourceType =typeof(SResource))]
    /// </code>
    /// </summary>
    public class DisplayExtAttribute : DisplayNameAttribute
    {
        /// <summary>
        /// 默认的名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 资源的Key，如果语言包有这个Key,优先使用
        /// </summary>
        public string ResName { get; set; }
        /// <summary>
        /// 资源类型
        /// </summary>
        public Type ResourceType { get; set; }
        /// <summary>
        /// 是否读取资源，默认True
        /// </summary>
        public bool IsReadRes { get; set; } = true;

        /// <summary>
        /// 默认警告方法
        /// </summary>
        public DisplayExtAttribute() { }

        /// <summary>
        /// 快速构建
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_resName"></param>
        /// <param name="_resType"></param>
        /// <param name="_isReadRes"></param>
        public DisplayExtAttribute(string _name, string _resName, Type _resType, bool _isReadRes = true)
        {
            this.Name = _name;
            this.ResName = _resName;
            this.ResourceType = _resType;
            this.IsReadRes = _isReadRes;
        }

        /// <summary>
        /// 重写显示名称
        /// </summary>
        public override string DisplayName
        {
            get
            {
                //会读取默认的语言包，加个开关防止读资源
                if (IsReadRes)
                {
                    if (!string.IsNullOrEmpty(ResName))
                    {
                        var lang = ""; //从系统中读当前语种
#if NETSTANDARD2_1
                        lang = UtilsBase.HttpContext.GetRouteValue("lang")?.ToString();
#else
                        var route = System.Web.HttpContext.Current.Request.RequestContext.RouteData;
                        if (route != null && route.Values.Keys.Contains("lang"))
                        {
                            lang = route.Values["lang"].ToString();
                        }
#endif

                        ResourceManager rm = new ResourceManager(ResourceType);
                        var resDisplayName = string.IsNullOrEmpty(lang)
                            ? rm.GetString(ResName)
                            : rm.GetString(ResName, new CultureInfo(lang));

                        if (!string.IsNullOrEmpty(resDisplayName))
                            return resDisplayName;
                    }
                }

                return Name;
            }
        }
    }
}
