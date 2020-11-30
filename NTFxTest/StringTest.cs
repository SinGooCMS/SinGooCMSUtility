using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;

namespace CoreTest
{
    [TestClass]
    public class StringTest
    {
        [TestMethod]
        public void TestRandomString()
        {
            StringBuilder builder = new StringBuilder();
            var lst = new List<string>();
            for (var i = 0; i < 100; i++)
            {
                if (i % 2 == 0)
                    builder.Append($"随机数：{StringUtils.GetNewFileName()}\r\n");
                else
                    builder.Append($"随机数：{StringUtils.GetRandomString()}\r\n");
            }

            Console.WriteLine(builder.ToString());
        }

        [TestMethod]
        public void TransJian()
        {
            //转简体
            string txtFan = "今天天氣不錯，后天可能會下雪";
            Assert.AreEqual(true, StringUtils.GetSimplified(txtFan).Contains("今天天气不错，后天可能会下雪"));
        }

        [TestMethod]
        public void TransFan()
        {
            //转繁体
            string txtJian = "今天天气不错，后天可能会下雪";
            Console.WriteLine(StringUtils.GetTraditional(txtJian)); //今天天氣不錯，后天可能會下雪
        }

        [TestMethod]
        public void TestPinyin()
        {
            string txtZHCN = "今天天气不错，后天可能会下雪";
            Console.WriteLine($"【今天天气不错，后天可能会下雪】的拼音：{StringUtils.GetChineseSpell(txtZHCN)} \r\n");
            Console.WriteLine($"【今天天气不错，后天可能会下雪】的拼音首字母：{StringUtils.GetChineseSpellFirst(txtZHCN)}\r\n");

        }
    }
}
