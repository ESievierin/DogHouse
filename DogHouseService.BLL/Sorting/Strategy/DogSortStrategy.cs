using DogHouseService.BLL.Extensions;
using DogHouseService.BLL.Interfaces.Sorting;
using DogHouseService.BLL.Models.Sorting;
using DogHouseService.DAL.DbModels;
using System.Linq.Expressions;

namespace DogHouseService.BLL.Sorting.Strategy
{
    public sealed class DogSortStrategy : ISortStrategy<DogDbModel>
    {
        public IOrderedQueryable<DogDbModel> Apply(IQueryable<DogDbModel> query, Enum? attribute, OrderByMode? orderByMode)
        {
            var sortAttribute = attribute is DogsOrderAttribute doa ? doa : DogsOrderAttribute.Name;
            var sortDir = orderByMode ?? OrderByMode.Asc;

            Expression<Func<DogDbModel, object>> expression = sortAttribute switch
            {
                DogsOrderAttribute.Name => d => d.Name,
                DogsOrderAttribute.Color => d => d.Color,
                DogsOrderAttribute.Weight => d => d.Weight,
                DogsOrderAttribute.Tail_length => d => d.Tail_length,
                _ => d => d.Name
            };

            return query.ApplyOrder(expression, sortDir);
        }
    }
}
