using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NUnit.Framework;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;

namespace CoreTest
{
    public class ExtensionTest
    {
        [Test]
        public void TestJson()
        {
            var user = new StudentInfo() { UserName = "jsonlee", Age = 18 };
            var json = user.ToJson();
            Console.WriteLine("user to json:" + json);
            var user2 = json.JsonToObject<StudentInfo>();
            Assert.AreEqual("jsonlee", user2.UserName);
        }

        [Test]
        public void TestJosnToAnanymous()
        {
            var json = "{\"UserName\":\"jsonlee\",\"Age\":18}";
            //匿名类，动态类
            var model = new { UserName = "", Age = 0 };
            var obj = json.JsonToAnonymousObject(model);
            Assert.AreEqual("jsonlee", obj.UserName);
        }

        [Test]
        public void TestXml()
        {
            var users = new List<StudentInfo>() {
                new StudentInfo() { UserName = "jsonlee", Age = 18 },
                new StudentInfo() { UserName = "jsonlee2", Age = 22 }
            };
            var xml = users.ToXml();
            Console.WriteLine("user to json:" + xml);
            var users2 = xml.XmlToObject<List<StudentInfo>>();
            Assert.AreEqual("jsonlee", users2[0].UserName);

        }

        [Test]
        public void TestXmlToObj()
        {
            //如果是list,根目录是这样的：ArrayOfStudent
            string xml = @"<?xml version='1.0' encoding='utf - 8'?>
                          <StudentInfo>
                            <score>100</score>
                            <AutoID>1</AutoID>
                            <UserName>jsonlee</UserName>
                            <Age>18</Age>
                            <Birthday>0001-01-01T00:00:00</Birthday>
                            <AutoTimeStamp>0001-01-01T00:00:00</AutoTimeStamp>
                          </StudentInfo> ";

            var obj = xml.XmlToObject<StudentInfo>();
            Assert.AreEqual(obj.UserName, "jsonlee");
        }

        [Test]
        public void TestDate()
        {
            var now = DateTime.Now;
            Console.WriteLine($"周一：{now.GetMonday()} 周日：{now.GetSunday()}");
            Console.WriteLine($"国外周第一天：{now.GetWeekFirstDay(false)} 国外周第最后一天：{now.GetWeekLastDay(false)}");
            Console.WriteLine($"中国周第一天：{now.GetWeekFirstDay()} 中国周第最后一天：{now.GetWeekLastDay()}");
            Console.WriteLine($"月初：{now.GetMonthFirstDay()} 月尾：{now.GetMonthLastDay()}");
            Console.WriteLine($"季初：{now.GetQuarterFirstDay()} 季尾：{now.GetQuarterLastDay()}");
            Console.WriteLine($"年初：{now.GetYearFirstDay()} 年尾：{now.GetYearLastDay()}");

            //注意 2020, 12, 31 和 2021, 1, 1是同一周，因为跨年了，分成了2个结果
            var date1 = new DateTime(2020, 12, 31);
            Console.WriteLine($"本年第几周：{date1.WeekOfYear()}"); //本年第几周：53
            var date2 = new DateTime(2021, 1, 1);
            Console.WriteLine($"本年第几周：{date2.WeekOfYear()}"); //本年第几周：1

            Console.WriteLine($"2020-12-31第几周：{date1.WeekOfYear(true)}；2021-1-1第几周：{date2.WeekOfYear(true)}"); //本年第几周：1

            Console.WriteLine($"格式化：{now.ToFormatDate()}");
            Console.WriteLine($"总秒数：{now.GetUnixTimeSeconds()}");
            Console.WriteLine($"当前中国时辰：{now.GetCNHour().DIZHI} - {now.GetCNHour().CNHOUR}");
        }

        [Test]
        public void ConvertTest()
        {
            var builder = new StringBuilder();
            builder.Append($"{"100"}转Long:{"100".ToLong()}\r\n");
            builder.Append($"{"100"}转Int:{"100".ToInt()}\r\n");
            builder.Append($"{"100"}转Short:{"100".ToShort()}\r\n");
            builder.Append($"{"100"}转Byte:{"100".ToByte()}\r\n");

            builder.Append($"{"100.89"}转Float:{"100.89".ToFloat()}\r\n");
            builder.Append($"{"100.89"}转Double:{"100.89".ToDouble()}\r\n");
            builder.Append($"{"100.89"}转Decimal:{"100.89".ToDecimal()}\r\n");

            builder.Append($"{"100.89"}转int:{"100.89".ToInt()}\r\n");
            builder.Append($"{100.89m}转byte:{100.89m.ToByte()}\r\n");
            builder.Append($"{100.89m}转short:{100.89m.ToShort()}\r\n");
            builder.Append($"{100.89m}转int:{100.89m.ToInt()}\r\n");
            builder.Append($"{100.89m}转long:{100.89m.ToLong()}\r\n");

            builder.Append($"{"true"}转bool:{"true".ToBool()}\r\n");

            builder.Append($"{"2020-12-31"}转datetime:{"2020-12-31".ToDateTime()}\r\n");
            builder.Append($"{"2020-12-31 12:00"}转datetime:{"2020-12-31 12:00".ToDateTime()}\r\n");

            Console.WriteLine(builder.ToString());

            var d = 18.5m;
            int e = d.To<int>(); //转换正确 但是18.9会转成19
            Assert.AreEqual(18, e);

            int f = d.ToInt(); //转换正确
            Assert.AreEqual(18, f);
        }

        [Test]
        public void ObjTest()
        {
            var user = new StudentInfo() { UserName = "jsonlee", Age = 18 };
            var user2 = user.DeepClone();
            Assert.AreEqual(user.UserName, user2.UserName);
            Assert.AreEqual(false, user2.ReferenceEquals(user)); //深度copy后，两个对象引用不同的内存空间
        }

        [Test]
        public void ValidTest()
        {
            var builder = new StringBuilder();
            builder.Append($"验证测试\r\n");
            builder.Append($" 123 is int:{"123".IsInt()}\r\n");
            builder.Append($" 123.5 is int:{"123.5".IsInt()}\r\n");
            builder.Append($" 123 is decimal:{"123".IsDecimal()}\r\n");
            builder.Append($" 123.5 is decimal:{"123.5".IsDecimal()}\r\n");

            builder.Append($" 16826375@163.com is email:{"16826375@163.com".IsEmail()}\r\n");
            builder.Append($" 17788760902 is mobile:{"17788760902".IsMobile()}\r\n");

            builder.Append($" jsonlee is en:{"jsonlee".IsEn()}\r\n");
            builder.Append($" 吕布 is cn:{"吕布".IsZHCN()}\r\n");

            builder.Append($" http://www.singoo.top is url:{"http://www.singoo.top".IsUrl()}\r\n");
            builder.Append($" http://www.singoo.top/article/news?p=1 is url:{"http://www.singoo.top/article/news?p=1".IsUrl()}\r\n");

            Console.WriteLine(builder.ToString());
        }

        [Test]
        public void StringTest()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("123转人民币：{0} \r\n", 123.9m.ToRMB());
            builder.AppendFormat("123456789转掩码：{0} \r\n", "123456789".Mask());

            string urlEncode = "http://www.singoo.top/?a=你好".AsEncodeUrl();
            builder.AppendFormat("http://www.singoo.top/?a=你好 URL编码：{0} \r\n", urlEncode);
            builder.AppendFormat("{0} URL解码：{1} \r\n", urlEncode, urlEncode.AsDecodeUrl());

            Console.WriteLine(builder.ToString());

            var dict = new Dictionary<string, string>();
            dict.Add("username", "刘备");
            dict.Add("age", "18");
            Console.WriteLine("url:" + dict.ToUrlSearch());

            var urlText = "username=%E5%88%98%E5%A4%87&age=18";
            Console.WriteLine("username:" + urlText.ToUrlDictionary()["username"]);
        }

        [Test]
        public void SpliterTest()
        {
            var str = "1,22,3,44,55";
            var arr = str.ToIntArray();
            Assert.AreEqual(5, arr.Length);

            var str2 = "true,false,true,true";
            var arr2 = str2.ToSpliterArray<bool>();
            Assert.AreEqual(true, arr2[3]);
        }
    }
}