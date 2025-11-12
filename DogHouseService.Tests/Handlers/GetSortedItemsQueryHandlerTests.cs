using DogHouseService.BLL.Interfaces.Sorting;
using DogHouseService.BLL.Models.Filters;
using DogHouseService.BLL.Models.Sorting;
using DogHouseService.BLL.RequestHandlers;
using DogHouseService.DAL.DbModels;
using DogHouseService.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DogHouseService.Tests.RequestHandlers
{
    public class GetSortedItemsQueryHandlerTests
    {
        private static DogHouseDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DogHouseDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DogHouseDbContext(options);
        }

        [Fact]
        public async Task Handle_Should_Use_SortStrategy_And_Return_Paginated_Results()
        {
            await using var dbContext = CreateInMemoryContext();

            var dogs = new[]
            {
                new DogDbModel { Name = "Charlie", Color = "White", Tail_length = 15, Weight = 30 },
                new DogDbModel { Name = "Buddy", Color = "Brown", Tail_length = 10, Weight = 25 },
                new DogDbModel { Name = "Alfie", Color = "Black", Tail_length = 8, Weight = 22 }
            };

            dbContext.Dogs.AddRange(dogs);
            await dbContext.SaveChangesAsync();

            var mockStrategy = new Mock<ISortStrategy<DogDbModel>>();
            mockStrategy
                .Setup(s => s.Apply(It.IsAny<IQueryable<DogDbModel>>(), It.IsAny<Enum?>(), It.IsAny<OrderByMode?>()))
                .Returns<IQueryable<DogDbModel>, Enum?, OrderByMode?>((q, _, _) =>
                    q.OrderBy(d => d.Name));

            var mockFactory = new Mock<ISortStrategyFactory>();
            mockFactory
                .Setup(f => f.GetProvider<DogDbModel>())
                .Returns(mockStrategy.Object);

            var handler = new GetSortedItemsQueryHandler<DogDbModel>(dbContext, mockFactory.Object);

            var query = new GetSortedItemsQuery<DogDbModel>(
                SortAttribute: DogsOrderAttribute.Name,
                Pagination: new FilterPagination { PageNumber = 0, PageSize = 2 },
                OrderByMode: OrderByMode.Asc
            );

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
            Assert.Equal("Alfie", result[0].Name);
            Assert.Equal("Buddy", result[1].Name);

            mockFactory.Verify(f => f.GetProvider<DogDbModel>(), Times.Once);
            mockStrategy.Verify(s => s.Apply(It.IsAny<IQueryable<DogDbModel>>(), DogsOrderAttribute.Name, OrderByMode.Asc), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Use_InitialQuery_When_Provided()
        {
            await using var dbContext = CreateInMemoryContext();

            var dogs = new[]
            {
                new DogDbModel { Name = "Charlie", Color = "White", Tail_length = 15, Weight = 30 },
                new DogDbModel { Name = "Buddy", Color = "Brown", Tail_length = 10, Weight = 25 },
                new DogDbModel { Name = "Alfie", Color = "Black", Tail_length = 8, Weight = 22 }
            };

            dbContext.Dogs.AddRange(dogs);
            await dbContext.SaveChangesAsync();

            var mockStrategy = new Mock<ISortStrategy<DogDbModel>>();
            mockStrategy
                .Setup(s => s.Apply(It.IsAny<IQueryable<DogDbModel>>(), It.IsAny<Enum?>(), It.IsAny<OrderByMode?>()))
                .Returns<IQueryable<DogDbModel>, Enum?, OrderByMode?>((q, _, _) => q.OrderBy(d => d.Weight));

            var mockFactory = new Mock<ISortStrategyFactory>();
            mockFactory
                .Setup(f => f.GetProvider<DogDbModel>())
                .Returns(mockStrategy.Object);

            var handler = new GetSortedItemsQueryHandler<DogDbModel>(dbContext, mockFactory.Object);

            var initialQuery = dbContext.Dogs.Where(d => d.Weight > 23);

            var query = new GetSortedItemsQuery<DogDbModel>(
                SortAttribute: DogsOrderAttribute.Weight,
                Pagination: null,
                OrderByMode: OrderByMode.Asc,
                InitialQuery: initialQuery
            );

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
            Assert.All(result, d => Assert.True(d.Weight > 23));
        }

        [Fact]
        public async Task Handle_Should_Return_All_When_No_Pagination()
        {
            await using var dbContext = CreateInMemoryContext();

            dbContext.Dogs.AddRange(
                new DogDbModel { Name = "A", Color = "Black", Tail_length = 5, Weight = 10 },
                new DogDbModel { Name = "B", Color = "White", Tail_length = 8, Weight = 20 }
            );
            await dbContext.SaveChangesAsync();

            var mockStrategy = new Mock<ISortStrategy<DogDbModel>>();
            mockStrategy
                .Setup(s => s.Apply(It.IsAny<IQueryable<DogDbModel>>(), It.IsAny<Enum?>(), It.IsAny<OrderByMode?>()))
                .Returns<IQueryable<DogDbModel>, Enum?, OrderByMode?>((q, _, _) => q.OrderBy(d => d.Name));

            var mockFactory = new Mock<ISortStrategyFactory>();
            mockFactory.Setup(f => f.GetProvider<DogDbModel>()).Returns(mockStrategy.Object);

            var handler = new GetSortedItemsQueryHandler<DogDbModel>(dbContext, mockFactory.Object);
            var query = new GetSortedItemsQuery<DogDbModel>();

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.Length);
        }
    }
}