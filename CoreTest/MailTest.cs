using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Threading.Tasks;

namespace CoreTest
{
    public class MailTest
    {
        [Test]
        public void SendMail()
        {
            var result = MailUtils.Create(new MailConfig()
            {
                ServMailSMTP = "smtp.qq.com",
                ServMailPort = 465,
                ServMailAccount = "16826375@qq.com",
                ServMailUserName = "16826375",
                ServMailUserPwd = "tmkrikmhpdgqbhie",
                ServMailIsSSL = true
            })
            .SendEmail("16826375@qq.com", "测试标题1111", "测试内容1111");

        }

        [Test]
        public async Task SendMail2()
        {
            var result = await MailUtils.Create(new MailConfig()
            {
                ServMailSMTP = "smtp.qq.com",
                ServMailPort = 465,
                ServMailAccount = "16826375@qq.com",
                ServMailUserName = "16826375",
                ServMailUserPwd = "tmkrikmhpdgqbhie",
                ServMailIsSSL = true
            })
            .SendEmailAsync("16826375@qq.com", "测试标题2222", "测试内容2222");

        }
    }
}
