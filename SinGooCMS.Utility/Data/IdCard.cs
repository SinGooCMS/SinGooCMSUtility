using System;
using System.Collections.Generic;
using System.Text;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 身份证工具
    /// <para>使得方法</para>
    /// <code>
    /// IdCardUtils.Parse('xxxx').Sex
    /// </code>
    /// </summary>
    public class IdCardUtils
    {
        #region 公共属性

        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardNo { get; internal set; }

        /// <summary>
        /// 行政区划(标准县级: 6位)
        /// </summary>
        public string RegionCode => CardNo.Substring(0, 6);

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; internal set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; internal set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDay { get; internal set; }

        #endregion

        private IdCardUtils() { }

        /// <summary>
        /// 解析身份证号
        /// </summary>
        /// <param name="cardno"></param>
        /// <returns></returns>
        public static IdCardUtils Parse(string cardno)
        {
            if (!cardno.IsIDCard())
                return null;

            var inst = new IdCardUtils();
            inst.CardNo = cardno;
            inst.parseSex();
            inst.parseAge();
            return inst;
        }

        #region helper        

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <returns> 1:男 2:女</returns>
        private void parseSex()
        {
            string tmp;
            tmp = this.CardNo.Substring(this.CardNo.Length - 4);
            tmp = tmp.Substring(0, 3);

            Math.DivRem(tmp.To<int>(), 2, out int outNum);
            this.Sex = outNum == 1 ? "男" : "女";
        }

        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <returns>年龄</returns>
        private void parseAge()
        {
            parseBrithday();
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - this.BirthDay.Year;
            // 再考虑月、天的因素
            if (nowDateTime.Month < this.BirthDay.Month || (nowDateTime.Month == this.BirthDay.Month && nowDateTime.Day < this.BirthDay.Day))
            {
                age--;
            }
            this.Age = age;
        }

        /// <summary>
        /// 解析生日
        /// </summary>
        /// <returns></returns>
        private void parseBrithday()
        {
            var rtn = this.CardNo.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            var birth = DateTime.Parse(rtn);
            this.BirthDay = birth;
        }

        #endregion
    }
}
