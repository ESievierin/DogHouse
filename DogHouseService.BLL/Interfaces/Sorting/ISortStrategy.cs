using DogHouseService.BLL.Models.Sorting;

namespace DogHouseService.BLL.Interfaces.Sorting
{
    public interface ISortStrategy<T>
    {
        IOrderedQueryable<T> Apply(IQueryable<T> query, Enum? attribute, OrderByMode? orderByMode);
    }
}
