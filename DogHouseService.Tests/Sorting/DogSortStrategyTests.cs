using DogHouseService.BLL.Models.Sorting;
using DogHouseService.BLL.Sorting.Strategy;
using DogHouseService.DAL.DbModels;
using DogHouseService.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace DogHouseService.Tests.Sorting
{
    public class DogSortStrategyTests
    {
        private DogHouseDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DogHouseDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new DogHouseDbContext(options);

            context.Dogs.AddRange(
                new DogDbModel { Name = "Charlie", Color = "White", Tail_length = 10, Weight = 25 },
                new DogDbModel { Name = "Buddy", Color = "Brown", Tail_length = 15, Weight = 30 },
                new DogDbModel { Name = "Max", Color = "Black", Tail_length = 5, Weight = 20 }
            );

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task Apply_Should_Sort_By_Name_Asc()
        {
            var context = CreateInMemoryContext();
            var strategy = new DogSortStrategy();

            var query = strategy.Apply(context.Dogs, DogsOrderAttribute.Name, OrderByMode.Asc);

            var result = await query.Select(d => d.Name).ToListAsync();

            Assert.Equal(["Buddy", "Charlie", "Max"], result);
        }

        [Fact]
        public async Task Apply_Should_Sort_By_Weight_Desc()
        {
            var context = CreateInMemoryContext();
            var strategy = new DogSortStrategy();

            var query = strategy.Apply(context.Dogs, DogsOrderAttribute.Weight, OrderByMode.Desc);

            var result = await query.Select(d => d.Weight).ToListAsync();

            Assert.Equal([30, 25, 20], result);
        }

        [Fact]
        public async Task Apply_Should_Sort_By_TailLength_Asc()
        {
            var context = CreateInMemoryContext();
            var strategy = new DogSortStrategy();

            var query = strategy.Apply(context.Dogs, DogsOrderAttribute.Tail_length, OrderByMode.Asc);

            var result = await query.Select(d => d.Tail_length).ToListAsync();

            Assert.Equal([5, 10, 15], result);
        }

        [Fact]
        public async Task Apply_Should_Default_To_Name_When_Unknown_Attribute()
        {
            var context = CreateInMemoryContext();
            var strategy = new DogSortStrategy();

            var query = strategy.Apply(context.Dogs, (DogsOrderAttribute)999, OrderByMode.Asc);

            var result = await query.Select(d => d.Name).ToListAsync();

            Assert.Equal(["Buddy", "Charlie", "Max"], result);
        }
    }
}
