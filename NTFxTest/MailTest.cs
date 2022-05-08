using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;
using System.ComponentModel.DataAnnotations.Schema;
using SinGooCMS.Ado.Interface;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace NTFxTest
{
    [TestClass]
    public class MailTest
    {
        [TestMethod]
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

        [TestMethod]
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
