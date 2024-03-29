﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 读取纯真IP数据库，定位IP地址
    /// </summary>
    public class IPScanner
    {
        #region 私有成员
        private string dataPath;
        private string ip;
        private string country;
        private string local;

        private long firstStartIp = 0;
        private long lastStartIp = 0;
        private FileStream objfs = null;
        private long startIp = 0;
        private long endIp = 0;
        private int countryFlag = 0;
        private long endIpOff = 0;
        private string errMsg = null;
        #endregion

        #region 构造函数

        /// <summary>
        /// IP位置查找器
        /// </summary>
        /// <param name="dbPath">纯真数据库地址，默认是：/ipdata/qqwry.dat</param>
        public IPScanner(string dbPath = "/ipdata/qqwry.dat")
        {
            dataPath = SystemUtils.GetMapPath(dbPath);
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 纯真数据库地址：绝对路径
        /// </summary>
        public string DataPath
        {
            set { dataPath = value; }
        }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP
        {
            set { ip = value; }
        }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country
        {
            get { return country; }
        }
        /// <summary>
        /// 地区
        /// </summary>
        public string Local
        {
            get { return local; }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg
        {
            get { return errMsg; }
        }
        #endregion

        #region 搜索匹配数据
        private int QQwry()
        {
            string pattern = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";
            Regex objRe = new Regex(pattern);
            Match objMa = objRe.Match(ip);
            if (!objMa.Success)
            {
                this.errMsg = "IP格式错误";
                return 4;
            }

            long ip_Int = this.IpToInt(ip);
            int nRet = 0;
            if (ip_Int >= IpToInt("127.0.0.0") && ip_Int <= IpToInt("127.255.255.255"))
            {
                this.country = "本机内部环回地址";
                this.local = "";
                nRet = 1;
            }
            else if ((ip_Int >= IpToInt("0.0.0.0") && ip_Int <= IpToInt("2.255.255.255")) || (ip_Int >= IpToInt("64.0.0.0") && ip_Int <= IpToInt("126.255.255.255")) || (ip_Int >= IpToInt("58.0.0.0") && ip_Int <= IpToInt("60.255.255.255")))
            {
                this.country = "网络保留地址";
                this.local = "";
                nRet = 1;
            }
            objfs = new FileStream(this.dataPath, FileMode.Open, FileAccess.Read);
            try
            {
                //objfs.Seek(0,SeekOrigin.Begin);
                objfs.Position = 0;
                byte[] buff = new Byte[8];
                objfs.Read(buff, 0, 8);
                firstStartIp = buff[0] + buff[1] * 256 + buff[2] * 256 * 256 + buff[3] * 256 * 256 * 256;
                lastStartIp = buff[4] * 1 + buff[5] * 256 + buff[6] * 256 * 256 + buff[7] * 256 * 256 * 256;
                long recordCount = Convert.ToInt64((lastStartIp - firstStartIp) / 7.0);
                if (recordCount <= 1)
                {
                    country = "FileDataError";
                    objfs.Close();
                    return 2;
                }
                long rangE = recordCount;
                long rangB = 0;
                long recNO = 0;
                while (rangB < rangE - 1)
                {
                    recNO = (rangE + rangB) / 2;
                    this.GetStartIp(recNO);
                    if (ip_Int == this.startIp)
                    {
                        rangB = recNO;
                        break;
                    }
                    if (ip_Int > this.startIp)
                        rangB = recNO;
                    else
                        rangE = recNO;
                }
                this.GetStartIp(rangB);
                this.GetEndIp();
                if (this.startIp <= ip_Int && this.endIp >= ip_Int)
                {
                    this.GetCountry();
                    this.local = this.local.Replace("（我们一定要解放台湾！！！）", "");
                }
                else
                {
                    nRet = 3;
                    this.country = "未知";
                    this.local = "";
                }
                objfs.Close();
                return nRet;
            }
            catch (Exception ex)
            {
                return 1;
            }

        }
        #endregion

        #region IP地址转换成Int数据
        private long IpToInt(string ip)
        {
            char[] dot = new char[] { '.' };
            string[] ipArr = ip.Split(dot);
            if (ipArr.Length == 3)
                ip = ip + ".0";
            ipArr = ip.Split(dot);

            long ip_Int = 0;
            long p1 = long.Parse(ipArr[0]) * 256 * 256 * 256;
            long p2 = long.Parse(ipArr[1]) * 256 * 256;
            long p3 = long.Parse(ipArr[2]) * 256;
            long p4 = long.Parse(ipArr[3]);
            ip_Int = p1 + p2 + p3 + p4;
            return ip_Int;
        }
        #endregion

        #region int转换成IP
        private string IntToIP(long ip_Int)
        {
            long seg1 = (ip_Int & 0xff000000) >> 24;
            if (seg1 < 0)
                seg1 += 0x100;
            long seg2 = (ip_Int & 0x00ff0000) >> 16;
            if (seg2 < 0)
                seg2 += 0x100;
            long seg3 = (ip_Int & 0x0000ff00) >> 8;
            if (seg3 < 0)
                seg3 += 0x100;
            long seg4 = (ip_Int & 0x000000ff);
            if (seg4 < 0)
                seg4 += 0x100;
            string ip = seg1.ToString() + "." + seg2.ToString() + "." + seg3.ToString() + "." + seg4.ToString();

            return ip;
        }
        #endregion

        #region 获取起始IP范围
        private long GetStartIp(long recNO)
        {
            long offSet = firstStartIp + recNO * 7;
            //objfs.Seek(offSet,SeekOrigin.Begin);
            objfs.Position = offSet;
            byte[] buff = new Byte[7];
            objfs.Read(buff, 0, 7);

            endIpOff = Convert.ToInt64(buff[4].ToString()) + Convert.ToInt64(buff[5].ToString()) * 256 + Convert.ToInt64(buff[6].ToString()) * 256 * 256;
            startIp = Convert.ToInt64(buff[0].ToString()) + Convert.ToInt64(buff[1].ToString()) * 256 + Convert.ToInt64(buff[2].ToString()) * 256 * 256 + Convert.ToInt64(buff[3].ToString()) * 256 * 256 * 256;
            return startIp;
        }
        #endregion

        #region 获取结束IP
        private long GetEndIp()
        {
            //objfs.Seek(endIpOff,SeekOrigin.Begin);
            objfs.Position = endIpOff;
            byte[] buff = new Byte[5];
            objfs.Read(buff, 0, 5);
            this.endIp = Convert.ToInt64(buff[0].ToString()) + Convert.ToInt64(buff[1].ToString()) * 256 + Convert.ToInt64(buff[2].ToString()) * 256 * 256 + Convert.ToInt64(buff[3].ToString()) * 256 * 256 * 256;
            this.countryFlag = buff[4];
            return this.endIp;
        }
        #endregion

        #region 获取国家/区域偏移量
        private string GetCountry()
        {
            switch (this.countryFlag)
            {
                case 1:
                case 2:
                    this.country = GetFlagStr(this.endIpOff + 4);
                    this.local = (1 == this.countryFlag) ? " " : this.GetFlagStr(this.endIpOff + 8);
                    break;
                default:
                    this.country = this.GetFlagStr(this.endIpOff + 4);
                    this.local = this.GetFlagStr(objfs.Position);
                    break;
            }
            return " ";
        }
        #endregion

        #region 获取国家/区域字符串
        private string GetFlagStr(long offSet)
        {
            int flag = 0;
            byte[] buff = new Byte[3];
            while (1 == 1)
            {
                //objfs.Seek(offSet,SeekOrigin.Begin);
                objfs.Position = offSet;
                flag = objfs.ReadByte();
                if (flag == 1 || flag == 2)
                {
                    objfs.Read(buff, 0, 3);
                    if (flag == 2)
                    {
                        this.countryFlag = 2;
                        this.endIpOff = offSet - 4;
                    }
                    offSet = Convert.ToInt64(buff[0].ToString()) + Convert.ToInt64(buff[1].ToString()) * 256 + Convert.ToInt64(buff[2].ToString()) * 256 * 256;
                }
                else
                {
                    break;
                }
            }
            if (offSet < 12)
                return " ";
            objfs.Position = offSet;
            return GetStr();
        }
        #endregion

        #region GetStr
        private string GetStr()
        {
            byte lowC = 0;
            byte upC = 0;
            string str = "";
            byte[] buff = new byte[2];
            while (1 == 1)
            {
                lowC = (Byte)objfs.ReadByte();
                if (lowC == 0)
                    break;
                if (lowC > 127)
                {
                    upC = (byte)objfs.ReadByte();
                    buff[0] = lowC;
                    buff[1] = upC;
                    System.Text.Encoding enc = System.Text.Encoding.GetEncoding("GB2312"); //.net core需要加入引用
                    str += enc.GetString(buff);
                }
                else
                {
                    str += (char)lowC;
                }
            }
            return str;
        }
        #endregion

        #region 获取IP地址

        /// <summary>
        /// IP定位
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        public string IPLocation(string ip = "", string dataPath = "")
        {
            if (!string.IsNullOrEmpty(dataPath))
                this.dataPath = dataPath;

            if (!string.IsNullOrEmpty(ip))
                this.ip = ip;

            if (string.IsNullOrEmpty(this.dataPath) || string.IsNullOrEmpty(ip))
                return string.Empty;
            else
            {
                this.QQwry();
                return this.country + this.local;
            }
        }

        #endregion

        #region 判断是否国内IP

        /// <summary>
        /// 是否国内IP，需要调用QQ纯真数据库，路径App_Data/qqwry.dat
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool IsChineseIP(string ip)
        {
            string[] chineseProvince = "北京,上海,天津,重庆,浙江,广东,江苏,河北,山西,四川,河南,辽宁,吉林,黑龙江,山东,安徽,福建,湖北,湖南,海南,江西,贵州,云南,陕西,甘肃,广西,宁夏,青海,新疆,西藏,内蒙古,香港,澳门,台湾".Split(',');
            IPLocation(ip);
            string ipCountry = this.Country ?? string.Empty;
            return chineseProvince.Where(p => ipCountry.Contains(p)).FirstOrDefault() != null;
        }

        #endregion
    }
}
