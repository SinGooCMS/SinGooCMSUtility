using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Web.Administration;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// IIS操作
    /// </summary>
    public class IISUtils
    {        
        #region 创建网站
        /// <summary> 
        /// 在本机的IIS创建Web网站
        /// </summary> 
        /// <param name="siteName">网站名</param> 
        /// <param name="bindingInfo">host <example>"*:80:myhost.com"</example></param> 
        /// <param name="physicalPath">网站路径</param> 
        /// <returns>成功返回True，失败返回false</returns>     
        public static bool CreateSite(string siteName, string bindingInfo, string physicalPath)
        {
            return CreateSite(siteName, "http", bindingInfo, physicalPath, true, siteName, ProcessModelIdentityType.ApplicationPoolIdentity, null, null, ManagedPipelineMode.Integrated, null);
        }
        /// <summary>
        /// 在本机的IIS创建Web网站
        /// </summary>
        /// <param name="siteName">网站名</param>
        /// <param name="protocol">http头</param>
        /// <param name="bindingInformation">网站例如<example>"*:80:www.sufeinet.com"</example></param>
        /// <param name="physicalPath">网站的路径</param>
        /// <param name="createAppPool">是否创建应用程序程序池</param>
        /// <param name="appPoolName">应用程序名</param>
        /// <param name="identityType">标识</param>
        /// <param name="appPoolUserName">用户名没有时用户名为Null即可</param>
        /// <param name="appPoolPassword">密码</param>
        /// <param name="appPoolPipelineMode">模式，经典还是集成</param>
        /// <param name="managedRuntimeVersion">.net版本</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool CreateSite(string siteName, string protocol, string bindingInformation, string physicalPath, bool createAppPool, string appPoolName,
            ProcessModelIdentityType identityType, string appPoolUserName, string appPoolPassword, ManagedPipelineMode appPoolPipelineMode, string managedRuntimeVersion)
        {
            using (ServerManager mgr = new ServerManager())
            {
                //删除网站和应用程序池
                DeleteSite(siteName);
                Site site = mgr.Sites.Add(siteName, protocol, bindingInformation.ToLower(), physicalPath);

                // PROVISION APPPOOL IF NEEDED 
                if (createAppPool)
                {
                    ApplicationPool pool = mgr.ApplicationPools.Add(appPoolName);
                    if (pool.ProcessModel.IdentityType != identityType)
                    {
                        pool.ProcessModel.IdentityType = identityType;
                    }
                    if (!String.IsNullOrEmpty(appPoolUserName))
                    {
                        pool.ProcessModel.UserName = appPoolUserName;
                        pool.ProcessModel.Password = appPoolPassword;
                    }
                    if (appPoolPipelineMode != pool.ManagedPipelineMode)
                    {
                        pool.ManagedPipelineMode = appPoolPipelineMode;
                    }
                    site.Applications["/"].ApplicationPoolName = pool.Name;
                }
                if (site != null)
                {
                    mgr.CommitChanges();
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 删除网站和应用程序池
        /// <summary> 
        /// 删除网站包括应用程序池
        /// </summary> 
        /// <param name="siteName">网站名</param> 
        /// <param name="isAppPool">是否删除应用程序池默认为删除</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool DeleteSite(string siteName, Boolean isAppPool = true)
        {
            using (ServerManager mgr = new ServerManager())
            {
                //判断web应用程序是否存在
                if (mgr.Sites[siteName] != null)
                {
                    if (isAppPool)
                    {
                        //判断应用程序池是否存在
                        if (mgr.ApplicationPools[siteName] != null)
                        {
                            mgr.ApplicationPools.Remove(mgr.ApplicationPools[siteName]);
                        }
                    }
                    mgr.Sites.Remove(mgr.Sites[siteName]);
                    mgr.CommitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary> 
        /// 删除应用程序池
        /// </summary> 
        /// <param name="appPoolName">应用程序池名</param> 
        /// <returns>成功返回True，失败返回false</returns>
        public static bool DeletePool(string appPoolName)
        {
            using (ServerManager mgr = new ServerManager())
            {
                ApplicationPool pool = mgr.ApplicationPools[appPoolName];
                if (pool != null)
                {
                    mgr.ApplicationPools.Remove(pool);
                    mgr.CommitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region 默认文档 添加和删除
        /// <summary>
        /// 添加默认文档
        /// </summary>
        /// <param name="siteName">网站</param>
        /// <param name="defaultDocName">默认文档名</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool AddDefaultDocument(string siteName, string defaultDocName)
        {
            using (ServerManager mgr = new ServerManager())
            {
                Configuration cfg = mgr.GetWebConfiguration(siteName);
                ConfigurationSection defaultDocumentSection = cfg.GetSection("system.webServer/defaultDocument");
                ConfigurationElement filesElement = defaultDocumentSection.GetChildElement("files");
                ConfigurationElementCollection filesCollection = filesElement.GetCollection();

                foreach (ConfigurationElement elt in filesCollection)
                {
                    if (elt.Attributes["value"].Value.ToString() == defaultDocName)
                    {
                        return false;//添加时存在
                    }
                }
                try
                {
                    //创建一个新的默认页
                    ConfigurationElement docElement = filesCollection.CreateElement();
                    docElement.SetAttributeValue("value", defaultDocName);
                    filesCollection.Add(docElement);
                }
                catch (Exception)
                {
                    return false;//添加时发生错误 
                }

                mgr.CommitChanges();
            }
            return true;//添加成功
        }
        /// <summary>
        /// 删除默认文档
        /// </summary>
        /// <param name="siteName">网站</param>
        /// <param name="defaultDocName">默认文档名</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool DeleteDefaultDocument(string siteName, string defaultDocName)
        {
            using (ServerManager mgr = new ServerManager())
            {
                Configuration cfg = mgr.GetWebConfiguration(siteName);
                ConfigurationSection defaultDocumentSection = cfg.GetSection("system.webServer/defaultDocument");
                ConfigurationElement filesElement = defaultDocumentSection.GetChildElement("files");
                ConfigurationElementCollection filesCollection = filesElement.GetCollection();

                //创建一个新的默认页
                ConfigurationElement docElement = filesCollection.CreateElement();

                bool isdefault = false;
                //不存在则返回
                foreach (ConfigurationElement elt in filesCollection)
                {
                    if (elt.Attributes["value"].Value.ToString() == defaultDocName)
                    {
                        docElement = elt;
                        isdefault = true;
                    }
                }
                if (!isdefault)
                {
                    return false;//不存在
                }
                try
                {
                    filesCollection.Remove(docElement);
                }
                catch (Exception)
                {
                    return false;//删除时发生错误 
                }

                mgr.CommitChanges();
            }
            return true;//删除成功
        }
        #endregion

        #region 虚拟目录添加和删除

        /// <summary>
        /// 添加虚拟目录
        /// </summary>
        /// <param name="siteName">网站名</param>
        /// <param name="vDirName">目录名</param>
        /// <param name="physicalPath">对应的文件夹路径</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool CreateVDir(string siteName, string vDirName, string physicalPath)
        {
            using (ServerManager mgr = new ServerManager())
            {
                Site site = mgr.Sites[siteName];
                if (site == null)
                {
                    return false;
                }
                site.Applications.Add("/" + vDirName, physicalPath);
                mgr.CommitChanges();
            }
            return true;
        }
        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="siteName">网站名</param>
        /// <param name="vDirName">目录名</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool DeleteVDir(string siteName, string vDirName)
        {
            using (ServerManager mgr = new ServerManager())
            {
                Site site = mgr.Sites[siteName];
                if (site == null)
                {
                    return false;
                }
                site.Applications.Remove(site.Applications["/" + vDirName]);
                mgr.CommitChanges();
            }
            return true;
        }
        #endregion

        #region 应用程序池暂停，启动
        /// <summary>
        /// 启动应用程序池
        /// </summary>
        /// <param name="poolName">应用程序池名</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool PoolsStart(string poolName)
        {
            try
            {
                using (ServerManager mgr = new ServerManager())
                {
                    if (mgr.ApplicationPools[poolName].State == ObjectState.Stopped)
                    {
                        mgr.ApplicationPools[poolName].Start();
                    }
                    mgr.CommitChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 暂停应用程序池
        /// </summary>
        /// <param name="poolName">应用程序池名</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool PoolsStop(string poolName)
        {
            try
            {
                using (ServerManager mgr = new ServerManager())
                {
                    if (mgr.ApplicationPools[poolName].State == ObjectState.Started)
                    {
                        mgr.ApplicationPools[poolName].Stop();
                    }
                    mgr.CommitChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 网站暂停，启动
        /// <summary>
        /// 启动网站
        /// </summary>
        /// <param name="siteName">网站名</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool Start(string siteName)
        {
            try
            {
                using (ServerManager mgr = new ServerManager())
                {
                    if (mgr.Sites[siteName].State == ObjectState.Stopped)
                    {
                        mgr.Sites[siteName].Start();
                    }
                    mgr.CommitChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 暂停网站
        /// </summary>
        /// <param name="siteName">网站名</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool Stop(string siteName)
        {
            try
            {
                using (ServerManager mgr = new ServerManager())
                {
                    if (mgr.Sites[siteName].State == ObjectState.Started)
                    {
                        mgr.Sites[siteName].Stop();
                    }
                    mgr.CommitChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 获取网站的信息
        /// <summary>
        /// 根据网站名获取网站信息
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public static Site GetSiteInfo(string siteName)
        {
            using (ServerManager mgr = new ServerManager())
            {
                return mgr.Sites[siteName];
            }
        }
        /// <summary>
        /// 获取所有的网站数据
        /// </summary>
        /// <returns>SiteCollection</returns>
        public static SiteCollection GetSiteList()
        {
            using (ServerManager mgr = new ServerManager())
            {
                return mgr.Sites;
            }
        }
        #endregion

        #region 添加绑定信息
        /// <summary>
        /// 添加绑定信息
        /// </summary>
        /// <param name="siteName">网站名</param>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
        /// <param name="domain">domain</param>
        /// <param name="bindingProtocol">协议头如 http/https默认为http</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool AddHostBinding(string siteName, string ip, string port, string domain, string bindingProtocol = "http")
        {
            try
            {
                using (ServerManager mgr = new ServerManager())
                {
                    string binginfo = string.Format("{0}:{1}:{2}", ip, port, domain).ToLower();
                    BindingCollection binding = mgr.Sites[siteName].Bindings;
                    binding.Add(binginfo, bindingProtocol);
                    mgr.CommitChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 删除绑定信息
        /// </summary>
        /// <param name="siteName">网站名</param>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
        /// <param name="domain">domain</param>
        /// <returns>成功返回True，失败返回false</returns>
        public static bool DeleteHostBinding(string siteName, string ip, string port, string domain)
        {
            try
            {
                using (ServerManager mgr = new ServerManager())
                {
                    string binginfo = string.Format("{0}:{1}:{2}", ip, port, domain).ToLower();
                    foreach (Binding item in mgr.Sites[siteName].Bindings)
                    {
                        if (item.BindingInformation == binginfo)
                        {
                            mgr.Sites[siteName].Bindings.Remove(item);
                            break;
                        }
                    }
                    mgr.CommitChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
