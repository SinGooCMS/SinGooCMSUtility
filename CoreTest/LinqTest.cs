using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SinGooCMS.Utility.Extension;

namespace CoreTest
{
    public class LinqTest
    {
        [Test]
        public async Task TestOrderby()
        {
            DbContext dbo = new DataContext();
            DbSet<StudentInfo> dbSet = dbo.Set<StudentInfo>();

            var lst1 = await dbSet.Where(p => p.Age > 35).OrderByBatch("UserName desc,Age desc").ToListAsync();
            Console.WriteLine(lst1.ToJson() + "\r\n");

            var source = await dbSet.ToListAsync();
            var lst2 = source.OrderByBatch("UserName desc,Age desc").ToList();
            Console.WriteLine(lst2.ToJson());
        }
    }

    public class DataContext : DbContext
    {
        public DbSet<StudentInfo> Student { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("server=(local);database=TestDB;uid=sa;pwd=123;");
        }
    }
}
