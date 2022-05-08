using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SinGooCMS.Utility
{
    /// <summary>
    ///对 Lambda 表达式进行动态拼接的逻辑
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// 默认条件成立
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary>
        /// 默认条件不成立
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary>
        /// 拼接多个Lambda 表达式，组成一个Lambda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"> 第一个lambda表达式</param>
        /// <param name="second"> 第二个lambda表达式</param>
        /// <param name="merge">合并的类型，AND，OR，NOT 等</param>
        /// <returns></returns>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            //uild parameter map (from parameters of second to parameters of first)   
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            //eplace parameters in the second lambda expression with parameters from the first   
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            //pply composition of lambda expression bodies to parameters from the first expression    
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// 两个表达式做And合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        /// <summary>
        /// 两个表达式做Or合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }

    internal class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
}
