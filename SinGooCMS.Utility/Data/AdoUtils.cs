using System;
using System.Collections.Generic;
using System.Text;
using SinGooCMS.Utility.Extension;
using System.Linq;

#if NETSTANDARD2_1
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 这里只提供SqlServer的简单操作，具体可参考SinGooCMS.Ado
    /// </summary>
    public static class AdoUtils
    {
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static bool IsConnected(string connStr)
        {
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                return conn.State==System.Data.ConnectionState.Open;
            }
        }

        /// <summary>
        /// 从模型中快速产生参数
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="model">实例</param>
        /// <param name="dictParamKeys">键值对，键=model模型的名称,值(值1=存储过程等的参数名称,值2=是否输出参数)</param>
        /// <returns></returns>
        public static SqlParameter[] ToSqlParams<T>(this T model, Dictionary<string, (string SqlParamName, bool IsOutput)> dictParamKeys) where T : class, new()
        {
            if (dictParamKeys != null && dictParamKeys.Count > 0)
            {
                var parameters = new List<SqlParameter>();
                var pi = typeof(T).GetProperties();                

                foreach (KeyValuePair<string, (string, bool)> item in dictParamKeys)
                {
                    var param = new SqlParameter();
                    param.ParameterName = item.Value.Item1;

                    var pi_param = pi.Where(p => p.Name == item.Key).FirstOrDefault();
                    if (pi_param != null)
                    {
                        switch (pi_param.PropertyType.Name)
                        {
                            case "Byte":
                                param.Value = model.GetPropertyVal<byte>(item.Key);
                                break;
                            case "Int16":
                                param.Value = model.GetPropertyVal<short>(item.Key);
                                break;
                            case "Int32":
                                param.Value = model.GetPropertyVal<int>(item.Key);
                                break;
                            case "Int64":
                                param.Value = model.GetPropertyVal<long>(item.Key);
                                break;
                            case "Single":
                                param.Value = model.GetPropertyVal<float>(item.Key);
                                break;
                            case "Double":
                                param.Value = model.GetPropertyVal<double>(item.Key);
                                break;
                            case "Decimal":
                                param.Value = model.GetPropertyVal<decimal>(item.Key);
                                break;
                            case "Boolean":
                                param.Value = model.GetPropertyVal<bool>(item.Key);
                                break;
                            case "DateTime":
                                param.Value = model.GetPropertyVal<DateTime>(item.Key);
                                break;
                            case "String":
                            default:
                                param.Value = model.GetPropertyVal<string>(item.Key);
                                break;
                        }
                    }

                    param.Direction = item.Value.Item2
                        ? System.Data.ParameterDirection.Output
                        : System.Data.ParameterDirection.Input;

                    parameters.Add(param);
                }

                return parameters.ToArray();
            }

            return null;
        }
    }
}
