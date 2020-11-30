using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;

namespace CoreTest
{
    public class StringTest
    {
        [Test]
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

            Assert.AreEqual(true, "{\"UserName\":\"jsonlee\"}".IsJson());

            Console.WriteLine(builder.ToString());
        }

        [Test]
        public void TransJian()
        {
            //转简体
            string txtFan = "今天天氣不錯，后天可能會下雪";
            Assert.AreEqual(true, StringUtils.GetSimplified(txtFan).Contains("今天天气不错，后天可能会下雪"));
        }

        [Test]
        public void TransFan()
        {
            //转繁体
            string txtJian = "今天天气不错，后天可能会下雪";
            Console.WriteLine(StringUtils.GetTraditional(txtJian)); //今天天氣不錯，后天可能會下雪
        }

        [Test]
        public void TestPinyin()
        {
            string txtZHCN = "今天天气不错，后天可能会下雪";
            Console.WriteLine($"【今天天气不错，后天可能会下雪】的拼音：{StringUtils.GetChineseSpell(txtZHCN)} \r\n");
            Console.WriteLine($"【今天天气不错，后天可能会下雪】的拼音首字母：{StringUtils.GetChineseSpellFirst(txtZHCN)}\r\n");

        }
    }
}
