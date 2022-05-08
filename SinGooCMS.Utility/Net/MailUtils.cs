using System;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using System.Threading.Tasks;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary> 
    /// <para>邮件发送工具 <br/>
    /// 来自: <seealso href="https://github.com/jstedfast/MimeKit"/>
    /// </para>
    /// <code>
    /// var result = await MailUtils.Create(new MailConfig()
    /// {
    ///      ServMailSMTP = "smtp.qq.com",
    ///      ServMailPort = 465,
    ///      ServMailAccount = "16826375@qq.com",
    ///      ServMailUserName = "16826375",
    ///      ServMailUserPwd = "这里是密码",
    ///      ServMailIsSSL = true
    /// })
    /// .SendEmailAsync("16826375@qq.com", "测试标题", "测试内容");    /// 
    /// </code>
    /// </summary> 
    public class MailUtils
    {
        /*
         * MailKit 是一款免费开源的.net邮件发送组件，支持跨平台，发送邮件主要使用这个组件
         * git：https://github.com/jstedfast/MailKit
         */

        private MailConfig Config { get; set; }

        /// <summary>
        /// 创建邮件客户端
        /// </summary>
        /// <param name="mailConfig"></param>
        /// <returns></returns>
        public static MailUtils Create(MailConfig mailConfig)
        {
            return new MailUtils
            {
                Config = mailConfig
            };
        }

        #region 发送邮件

        /// <summary>
        /// 使用SMTP协议，异步发送邮件
        /// </summary>
        /// <param name="rctMail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public (bool isSuccess, string errMsg) SendEmail(string rctMail, string subject, string body)
        {
            try
            {
                if (rctMail.Trim().Length == 0)
                    return (false, "没有找到收件箱地址");

                #region 发送邮件

                var message = new MimeMessage();
                var addrFrom = MailboxAddress.Parse(Config.ServMailAccount);
                if (!string.IsNullOrEmpty(Config.FromDisplayName))
                    addrFrom = new MailboxAddress(Config.FromDisplayName, Config.ServMailAccount);
                message.From.Add(addrFrom);

                foreach (string item in rctMail.Split(','))
                {
                    if (!string.IsNullOrEmpty(item) && item.IsEmail())
                    {
                        message.To.Add(MailboxAddress.Parse(item));
                    }
                }
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(Config.ServMailSMTP, Config.ServMailPort, Config.ServMailIsSSL);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(Config.ServMailUserName, Config.ServMailUserPwd);

                    client.Send(message);
                    client.Disconnect(true);

                    return (true, "");
                }

                #endregion
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// 使用SMTP协议，异步发送邮件
        /// </summary>
        /// <param name="rctMail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<(bool isSuccess, string errMsg)> SendEmailAsync(string rctMail, string subject, string body)
        {
            try
            {
                if (rctMail.Trim().Length == 0)
                    return (false, "没有找到收件箱地址");

                #region 发送邮件

                var message = new MimeMessage();
                var addrFrom = MailboxAddress.Parse(Config.ServMailAccount);
                if (!string.IsNullOrEmpty(Config.FromDisplayName))
                    addrFrom = new MailboxAddress(Config.FromDisplayName, Config.ServMailAccount);
                message.From.Add(addrFrom);

                foreach (string item in rctMail.Split(','))
                {
                    if (!string.IsNullOrEmpty(item) && item.IsEmail())
                    {
                        message.To.Add(MailboxAddress.Parse(item));
                    }
                }
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(Config.ServMailSMTP, Config.ServMailPort, Config.ServMailIsSSL);

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(Config.ServMailUserName, Config.ServMailUserPwd);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    return (true, "");
                }

                #endregion
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        #endregion    
    }

    /// <summary>
    /// 邮件服务配置
    /// </summary>
    public class MailConfig
    {
        /// <summary>
        /// SMTP服务器
        /// </summary>
        public string ServMailSMTP { get; set; }
        /// <summary>
        /// SMTP端口 默认25，为了保证安全，建议启用465等SSL端口
        /// </summary>
        public int ServMailPort { get; set; }
        /// <summary>
        /// 邮箱账户
        /// </summary>
        public string ServMailAccount { get; set; }
        /// <summary>
        /// 邮箱用户名
        /// </summary>
        public string ServMailUserName { get; set; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string ServMailUserPwd { get; set; }
        /// <summary>
        /// 发送方别名，这时使用站点名称
        /// </summary>
        public string FromDisplayName { get; set; }
        /// <summary>
        /// 是否启用SSL，启用用使用465端口/否则使用25端口
        /// </summary>
        public bool ServMailIsSSL { get; set; }
    }
}
