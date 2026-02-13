using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dorisoy.Pan.Common
{
    /// <summary>
    /// EF Core 10 + MySql.EntityFrameworkCore 兼容性扩展方法。
    /// 将 Contains 查询转换为 OR 谓词 (property = val1 OR property = val2 OR ...)，
    /// 绕过 MySQL 提供程序对 EF.Constant + Contains 的类型映射问题。
    /// </summary>
    public static class QueryableContainsExtensions
    {
        /// <summary>
        /// 替代 list.Contains(property) 的 EF Core 查询方式。
        /// 生成 SQL: WHERE property = @p0 OR property = @p1 OR ...
        /// </summary>
        public static IQueryable<TEntity> WhereContains<TEntity, TKey>(
            this IQueryable<TEntity> query,
            Expression<Func<TEntity, TKey>> propertySelector,
            IEnumerable<TKey> values)
        {
            var valuesList = values?.ToList() ?? new List<TKey>();
            if (valuesList.Count == 0)
                return query.Where(_ => false);

            var parameter = propertySelector.Parameters[0];
            Expression body = null;

            foreach (var value in valuesList)
            {
                var constant = Expression.Constant(value, typeof(TKey));
                var equal = Expression.Equal(propertySelector.Body, constant);
                body = body == null ? equal : Expression.OrElse(body, equal);
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(body!, parameter);
            return query.Where(lambda);
        }
    }
}
