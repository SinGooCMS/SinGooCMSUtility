using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;

namespace NTFxTest
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

            Assert.AreEqual(true, "{\"UserName\":\"jsonlee\"}".IsJson());

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

        [TestMethod]
        public void TestOther()
        {
            Console.WriteLine("转人民币：" + 135.83m.ToRMB()); //转人民币：壹佰叁拾伍圆捌角叁分
            Console.WriteLine("掩码：" + "17788760902".Mask()); //掩码：177****0902
            Assert.AreEqual(" 我是 江 西人".RemoveWhiteSpace(), "我是江西人");
            Console.WriteLine($"6位纯数字验证码:{StringUtils.GetRandomNumber(6)}");

            //正则分割
            string str = "今天天气可能不错，后天可能会下雪，年前可能出太阳";
            Console.WriteLine(StringUtils.Split(str, "可能").ToJson());

            string[] array = { "1","2","34","5"};
            Console.WriteLine("数组转字符串:" + array.ToSplitterString());

            //判断是否包含
            Assert.AreEqual(false, "1,2,34,5".ContainsWithSplitter("3"));
            Assert.AreEqual(true, "1,2,34,5".ContainsWithSplitter("5"));
        }

        [TestMethod]
        public void TestText()
        {
            var encode36 = DEncryptUtils.Base36Encode(123);
            Console.WriteLine(encode36); //3F

            var decode36= DEncryptUtils.Base36Decode(encode36);
            Console.WriteLine(decode36); //123

            var hex = HexUtils.ToHex("123");
            Console.WriteLine(hex); //313233

            var normal = HexUtils.GetStrFromHex(hex);
            Console.WriteLine(normal); //123

            var url = "http://www.singoo.top/?time=2022-05-01 10:00:00";
            var unicode=UnicodeUtils.Encode(url);
            Console.WriteLine(unicode); //\u0068\u0074\u0074\u0070\u003a\u002f\u002f\u0077\u0077\u0077\u002e\u0073\u0069\u006e\u0067\u006f\u006f\u002e\u0074\u006f\u0070\u002f\u003f\u0074\u0069\u006d\u0065\u003d\u0032\u0030\u0032\u0032\u002d\u0030\u0035\u002d\u0030\u0031\u0020\u0031\u0030\u003a\u0030\u0030\u003a\u0030\u0030

            var deUnicode = UnicodeUtils.Decode(unicode);
            Console.WriteLine(deUnicode); //http://www.singoo.top/?time=2022-05-01 10:00:00
        }
    }
}
