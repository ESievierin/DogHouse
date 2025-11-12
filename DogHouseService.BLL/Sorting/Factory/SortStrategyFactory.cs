using DogHouseService.BLL.Interfaces.Sorting;
using Microsoft.Extensions.DependencyInjection;

namespace DogHouseService.BLL.Sorting.Factory
{
    public sealed class SortStrategyFactory(IServiceProvider serviceProvider) : ISortStrategyFactory
    {
        public ISortStrategy<T> GetProvider<T>() =>
            serviceProvider.GetService<ISortStrategy<T>>();
    }
}
