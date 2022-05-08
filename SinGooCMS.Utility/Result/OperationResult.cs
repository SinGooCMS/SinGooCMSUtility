using System;
using Newtonsoft.Json.Linq;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 操作返回结果<br/>
    /// <exsample>
    /// "ReturnStatusType": 1,                —返回状态类型<br/>
    /// "StatusCode": "200",                  —错误信息代码<br/>
    /// "ErrorMessage": "操作成功",           —错误信息<br/>
    /// "ErrorENMessage": "ok",               —英文错误信息<br/>
    /// "TimeStamp": "2020-01-06 12:13:14",   —当前时间
    /// </exsample>
    /// </summary>
    public class OperationResult
    {
        #region 公共属性

        /// <summary>
        /// 返回状态类型 1 处理成功 0 处理失败 ,默认是失败的
        /// </summary>
        public int ReturnStatusType { get; set; } = 0;
        /// <summary>
        /// 错误信息代码
        /// </summary>
        public string StatusCode { get; set; } = string.Empty;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
        /// <summary>
        /// 错误信息(英文)
        /// </summary>
        public string ErrorENMessage { get; set; } = string.Empty;
        /// <summary>
        /// 当前时间,可不填，默认当前时间
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        /// <summary>
        /// 回调Url
        /// </summary>
        public string CallbackUrl { get; set; } = string.Empty;
        /// <summary>
        /// 附加数据
        /// </summary>
        public string Data { get; set; } = string.Empty;
        /// <summary>
        /// 超时时间(秒) 超时时间=TimeStamp.AddSecond(Timeout)
        /// </summary>
        public int Timeout { get; set; } = -1; //-1表示不限制

        private string _sign=string.Empty;
        /// <summary>
        /// 签名，用于验证数据是正确性，Sign=DEncryptUtils.DESEncrypt("ReturnStatusType=1&amp;StatusCode=200&amp;TimeStamp=2022-04-01 10:00:00&amp;Timeout=-1",key) <br/>
        /// key="singoo" + this.TimeStamp.ToString("yyyyMMdd");
        /// </summary>
        public string Sign 
        {
            get
            {
                //如果没有指定签名，给个默认签名
                string _sign = this._sign ?? string.Empty;
                if (_sign.Trim() == "")
                {
                    string key = "singoo" + this.TimeStamp.ToString("yyyyMMdd");
                    _sign = DEncryptUtils.DESEncrypt($"ReturnStatusType={this.ReturnStatusType}&StatusCode={this.StatusCode}&TimeStamp={this.TimeStamp}&Timeout={this.Timeout}", key);
                }

                return _sign;
            }
            set
            {
                _sign = value;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 无参构造方法
        /// </summary>
        public OperationResult() { }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_ReturnStatusType">状态 1/0</param>
        /// <param name="_StatusCode">状态码 如200，401，404，500</param>
        /// <param name="_ErrorMessage">中文提示</param>
        /// <param name="_ENErrorMessage">英文提示</param>
        public OperationResult(int _ReturnStatusType, string _StatusCode, string _ErrorMessage, string _ENErrorMessage = "")
        {
            this.ReturnStatusType = _ReturnStatusType;
            this.StatusCode = _StatusCode;
            this.ErrorMessage = _ErrorMessage;
            this.ErrorENMessage = _ENErrorMessage;
        }

        #endregion

        #region 快速返回结果

        public static OperationResult success = new OperationResult(1, "200", "操作成功", "ok");
        public static OperationResult Success(string errorMessage, string errorENMessage = "") => new OperationResult(1, "200", errorMessage, errorENMessage);

        public static OperationResult fail = new OperationResult(0, "－1", "操作失败", "fail");
        public static OperationResult Fail(string statusCode, string errorMessage, string errorENMessage = "") => new OperationResult(0, statusCode, errorMessage, errorENMessage);

        #endregion

        /// <summary>
        /// 返回一个json字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{{\"ReturnStatusType\":{ReturnStatusType},\"StatusCode\":\"{StatusCode}\",\"ErrorMessage\":\"{ErrorMessage}\",\"ErrorENMessage\":\"{ErrorENMessage}\",\"TimeStamp\":\"{TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\",\"CallbackUrl\":\"{CallbackUrl}\",\"Data\":\"{Data}\",\"Timeout\":\"{Timeout}\",\"Sign\":\"{Sign}\"}}";
        }

        /// <summary>
        /// 返回一个json对象
        /// </summary>
        /// <returns></returns>
        public JObject ToJsonObj()
        {
            var json = this.ToString();
            if (!string.IsNullOrEmpty(json))
                return JObject.Parse(json);

            return null;
        }
    }
}