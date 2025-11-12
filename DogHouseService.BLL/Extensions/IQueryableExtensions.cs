using DogHouseService.BLL.Models.Sorting;
using System.Linq.Expressions;

namespace DogHouseService.BLL.Extensions
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> ApplyOrder<T>(
            this IQueryable<T> query,
            Expression<Func<T,object>> keySelector,
            OrderByMode orderByMode)
        {
            return orderByMode switch
            {
                OrderByMode.Asc => query.OrderBy(keySelector),
                OrderByMode.Desc => query.OrderByDescending(keySelector),
                _ => query.OrderBy(keySelector)
            };
        }
    }
}
