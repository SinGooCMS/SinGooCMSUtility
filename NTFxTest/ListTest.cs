using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SinGooCMS.Utility.Extension;

namespace NTFxTest
{
    [TestClass]
    public class ListTest
    {
        [TestMethod]
        public void TestForeach()
        {
            //遍历
            var group = new string[] { "json", "lee", "123" }; //实现了
            group.ForEach(p => { Console.WriteLine($"元素：{p}"); });
        }

        [TestMethod]
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
    }
}
