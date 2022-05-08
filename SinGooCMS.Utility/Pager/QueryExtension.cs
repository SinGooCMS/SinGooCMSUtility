using System;
using System.Collections.Generic;
using System.Linq;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 查询并分页
    /// </summary>
    public static class QueryExtension
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageIndex));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            return pageIndex == 1
                ? query.Take(pageSize)
                : query.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }

        /// <summary>
        /// 分页查询并返回分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagerModel<IEnumerable<T>> PageModel<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageIndex));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            return new PagerModel<IEnumerable<T>>()
            {
                PagerData = pageIndex == 1
                            ? query.Take(pageSize).ToList()
                            : query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecord = query.Count()
            };

        }
    }
}
