namespace DogHouseService.BLL.Interfaces.Sorting
{
    public interface ISortStrategyFactory
    {
        ISortStrategy<T> GetProvider<T>();
    }
}
