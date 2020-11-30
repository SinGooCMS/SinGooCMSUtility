using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 执行系统进程
    /// </summary>
    public class ProcessUtils
    {
        /*  命令列:rundll32.exe user.exe,restartwindows 
            功能: 系统重启
            命令列:rundll32.exe user.exe,exitwindows
            功能: 关闭系统

            cmd关机命令 shutdown -s -t 60
            cmd取消关机 shutdown -a
         */

        #region 服务

        /// <summary>
        /// 开启服务（需要管理员权限）
        /// </summary>
        /// <param name="serviceName"></param>
        public static void StartService(string serviceName) =>
            ExecuteCMD(new List<string>() { $"sc start {serviceName}" });

        /// <summary>
        /// 停止服务（需要管理员权限）
        /// </summary>
        /// <param name="serviceName"></param>
        public static void StopService(string serviceName) =>
            ExecuteCMD(new List<string>() { $"sc stop {serviceName}" });

        /// <summary>
        /// 删除服务（需要管理员权限）
        /// </summary>
        /// <param name="serviceName"></param>
        public static void DeleteService(string serviceName) =>
            ExecuteCMD(new List<string>() { $"sc delete {serviceName}" });

        #endregion

        #region cmd命令

        /// <summary>
        /// 关机
        /// </summary>
        /// <param name="shutSecond">xx秒后关机，默认是60秒</param>
        public static void Shutdown(int shutSecond = 60) =>
            ExecuteCMD(new List<string>() { "shutdown -s -t " + shutSecond.ToString() });

        /// <summary>
        /// 取消关机
        /// </summary>
        public static void UnShutdown() =>
            ExecuteCMD(new List<string>() { "shutdown -a " });

        /// <summary>
        /// 执行cmd命令
        /// </summary>
        /// <param name="commandText"></param>
        public static bool ExecuteCMD(IList<string> commandText)
        {
            string result = "";
            string errMsg = "";
            return ExecuteCMD(commandText, out result, out errMsg);
        }

        /// <summary>
        /// 打开cmd执行一系列命令
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="result"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool ExecuteCMD(IList<string> commandText, out string result, out string errMsg)
        {
            return Execute("cmd", null, commandText, out result, out errMsg);
        }

        #endregion        

        #region 执行程序 

        /// <summary>
        /// 执行程序
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void Execute(string fileName, string args)
        {
            string result = "";
            string errMsg = "";
            Execute(fileName, args, null, out result, out errMsg);
        }

        /// <summary>
        /// 执行程序
        /// </summary>
        /// <param name="fileName">程序名称</param>
        /// <param name="args">参数</param>
        /// <param name="commandText">执行的命令，如cmd的命令</param>
        /// <param name="result">返回结果</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>返回执行是否成功</returns>
        public static bool Execute(string fileName, string args, IList<string> commandText, out string result, out string errMsg)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.FileName = fileName;

                if (!string.IsNullOrEmpty(args))
                    process.StartInfo.Arguments = args;

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                if (commandText != null && commandText.Count > 0)
                {
                    foreach (var item in commandText)
                        process.StandardInput.WriteLine(item); //执行的命令

                    process.StandardInput.WriteLine("exit");
                }

                result = process.StandardOutput.ReadToEnd(); //执行命令的返回值

                while (!process.HasExited)
                {
                    process.WaitForExit(1000);
                }
                errMsg = process.StandardError.ReadToEnd();
                process.StandardError.Close();

                if (string.IsNullOrEmpty(errMsg))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                process.Close();
                process.Dispose();
            }
        }

        #endregion
    }
}
