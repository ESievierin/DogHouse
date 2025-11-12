using DogHouseService.BLL.Interfaces.Sorting;
using DogHouseService.BLL.Sorting.Factory;
using DogHouseService.BLL.Sorting.Strategy;
using DogHouseService.DAL.DbModels;
using Microsoft.Extensions.DependencyInjection;

namespace DogHouseService.Tests.Sorting
{
    public class SortStrategyFactoryTests
    {
        [Fact]
        public void GetProvider_Should_Return_Registered_Strategy()
        {
            var services = new ServiceCollection();
            services.AddScoped<ISortStrategy<DogDbModel>, DogSortStrategy>();
            var provider = services.BuildServiceProvider();

            var factory = new SortStrategyFactory(provider);

            var strategy = factory.GetProvider<DogDbModel>();

            Assert.NotNull(strategy);
            Assert.IsType<DogSortStrategy>(strategy);
        }

        [Fact]
        public void GetProvider_Should_Return_Null_When_Not_Registered()
        {
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();

            var factory = new SortStrategyFactory(provider);

            var strategy = factory.GetProvider<DogDbModel>();

            Assert.Null(strategy);
        }
    }
}
