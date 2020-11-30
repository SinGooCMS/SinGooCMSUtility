using System;
using System.Text;
using System.Linq;
using NUnit.Framework;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;

namespace CoreTest
{
    public class ListTest
    {
        [Test]
        public void TestForeach()
        {
            //遍历
            var group = new string[] { "json", "lee", "123" }; //实现了
            group.ForEach(p => { Console.WriteLine($"元素：{p}"); });
        }

        [Test]
        public void TestDistinct()
        {
            //去重
            var group = new string[] { "json", "lee", "123", "123" };
            Console.WriteLine("group长度：" + group.Distinct().Count()); //linq去重

            var students = new Student[]{
                new Student(){ UserName="jsonlee",Age=18},
                new Student(){ UserName="jsonlee",Age=20}
            };
            Console.WriteLine("students长度：" + students.DistinctBy(p => p.UserName).Count());
        }
    }
}
