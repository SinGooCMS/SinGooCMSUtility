using System;
using System.Collections.Generic;
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

            //动态类型
            var students = new[]{
                new { UserName="jsonlee",Age=18},
                new { UserName="jsonlee",Age=20}
            };
            Console.WriteLine("students长度：" + students.DistinctBy(p => p.UserName).ToJson());

            var dt = students.Distinct().ToDataTable();
            Assert.AreEqual(true, dt.Rows.Count > 0);

        }

        [Test]
        public void TestHashSet()
        {
            //哈希集 散列集具有不重复的特性
            string[] arr = { "刘备", "关羽", "张飞", "小兵", "小兵", "小兵", "小兵", "小兵" };
            HashSet<string> hashSet = arr.ToHashSet();
            Console.WriteLine(hashSet.ToJson()+"\r\n"); //结果 ["刘备","关羽","张飞","小兵"]

            var arr2 = arr.ToHashSet(p => "小兵");
            Console.WriteLine(arr2.ToJson()); //结果 ["小兵"]
        }
    }
}
