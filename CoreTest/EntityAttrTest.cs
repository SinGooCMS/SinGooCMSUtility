using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoreTest
{
    public class EntityAttrTest
    {
        [Test]
        public void TestAttr()
        {
            var info = new AttrTestInfo() { AutoID = 1, UserName = "jsonlee", Password = "123" };

            var builder = new StringBuilder();
            builder.Append($"表名：{EntityAttrUtils.GetTableName(typeof(AttrTestInfo))} \r\n");
            builder.Append($"主键：{EntityAttrUtils.GetKey(typeof(AttrTestInfo))} \r\n");
            builder.Append($"主键值：{EntityAttrUtils.GetKeyValue(info)} \r\n");

            var pi1 = EntityAttrUtils.GetProperty<AttrTestInfo>("UserName");
            builder.Append($"最大值：{EntityAttrUtils.GetMaxLength(pi1)} \r\n");
            var pi2 = EntityAttrUtils.GetProperty<AttrTestInfo>("Password");
            builder.Append($"最小值：{EntityAttrUtils.GetMinLength(pi2)} \r\n");

            Console.WriteLine(builder.ToString());
        }
    }

    [Table("cms_Test")]
    public class AttrTestInfo
    {
        [Key]
        public int AutoID { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        [MinLength(6)]
        public string Password { get; set; }
    }
}
