using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;
using SinGooCMS.Utility;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;

namespace NTFxTest
{
    [TestClass]
    public class LinqTest
    {
        [TestMethod]
        public void TestLinq()
        {
            var lst = new List<StudentInfo> {
                new StudentInfo(){ AutoID=1, UserName="jasonlee",Age=18 },
                new StudentInfo(){ AutoID=2,UserName="刘备",Age=20},
                new StudentInfo(){ AutoID=3,UserName="刘备",Age=30}
            };

            var condition = PredicateBuilder.True<StudentInfo>();
            condition = condition.And(p => p.UserName == "刘备");
            condition = condition.And(p => p.Age == 30);
            var model = lst.AsQueryable().Where(condition).FirstOrDefault();

            Assert.AreEqual(3, model?.AutoID ?? 0);

            //排序
            var lst2 = lst.AsQueryable().OrderByBatch("UserName desc,Age desc").ToList();
            Assert.AreEqual(30, lst2[0].Age);

            //分页
            var lst3 = lst.AsQueryable().OrderByBatch("UserName desc,Age desc").Page(2, 2).ToList(); //每页2条记录，第2页
            Assert.AreEqual(18, lst3[0].Age);

            var pageModel = lst.AsQueryable().OrderByBatch("UserName desc,Age desc").PageModel(2, 2);
            Console.WriteLine($"pageIndex:{pageModel.PageIndex},pageSize:{pageModel.PageSize},totalPage:{pageModel.TotalPage}");
            Assert.IsTrue(pageModel.PagerData.Count()>0);
        }
    }
}
