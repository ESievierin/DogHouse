using AutoMapper;
using DogHouseService.BLL.DTO;
using DogHouseService.BLL.Models.Filters;
using DogHouseService.BLL.Models.Sorting;
using DogHouseService.BLL.RequestHandlers;
using DogHouseService.BLL.RequestHandlers.Dog;
using DogHouseService.DAL.DbModels;
using MediatR;
using Moq;

namespace DogHouseService.Tests.RequestHandlers
{
    public class GetAllSortedDogsQueryHandlerTests
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IMediator> mediatorMock;
        private readonly GetAllSortedDogsQueryHandler handler;

        public GetAllSortedDogsQueryHandlerTests()
        {
            mapperMock = new Mock<IMapper>();
            mediatorMock = new Mock<IMediator>();
            handler = new GetAllSortedDogsQueryHandler(mapperMock.Object, mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Call_Mediator_With_GetSortedItemsQuery_And_Return_Mapped_Dogs()
        {
            var dogQueryDto = new DogQueryDto
            {
                Pagination = new FilterPagination { PageNumber = 0, PageSize = 10 },
                OrderAttribute = DogsOrderAttribute.Name,
                OrderByMode = OrderByMode.Asc
            };

            var dbDogs = new[]
            {
                new DogDbModel { Name = "Buddy", Color = "Brown", Tail_length = 10, Weight = 25 },
                new DogDbModel { Name = "Charlie", Color = "White", Tail_length = 15, Weight = 30 }
            };

            var mappedDogs = new[]
            {
                new DogDto { Name = "Buddy", Color = "Brown", Tail_length = 10, Weight = 25 },
                new DogDto { Name = "Charlie", Color = "White", Tail_length = 15, Weight = 30 }
            };

            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetSortedItemsQuery<DogDbModel>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dbDogs);

            mapperMock
                .Setup(m => m.Map<DogDto[]>(It.IsAny<DogDbModel[]>()))
                .Returns(mappedDogs);

            var request = new GetAllSortedDogsQuery(dogQueryDto);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
            Assert.Equal("Buddy", result[0].Name);
            Assert.Equal("Charlie", result[1].Name);

            mediatorMock.Verify(m => m.Send(
                It.Is<GetSortedItemsQuery<DogDbModel>>(q =>
                    q.SortAttribute != null
                    && (DogsOrderAttribute)q.SortAttribute == DogsOrderAttribute.Name
                    && q.OrderByMode == OrderByMode.Asc
                    && q.Pagination != null
                    && q.Pagination.PageNumber == dogQueryDto.Pagination!.PageNumber
                    && q.Pagination.PageSize == dogQueryDto.Pagination!.PageSize
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);


            mapperMock.Verify(m => m.Map<DogDto[]>(It.Is<DogDbModel[]>(arr => arr.Length == dbDogs.Length)), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_Array_When_No_Dogs()
        {
            var dogQueryDto = new DogQueryDto
            {
                OrderAttribute = DogsOrderAttribute.Weight,
                OrderByMode = OrderByMode.Desc
            };

            var dbDogs = Array.Empty<DogDbModel>();
            var mappedDogs = Array.Empty<DogDto>();

            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetSortedItemsQuery<DogDbModel>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dbDogs);

            mapperMock
                .Setup(m => m.Map<DogDto[]>(It.IsAny<DogDbModel[]>()))
                .Returns(mappedDogs);

            var request = new GetAllSortedDogsQuery(dogQueryDto);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);

            mediatorMock.Verify(m => m.Send(It.IsAny<GetSortedItemsQuery<DogDbModel>>(), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<DogDto[]>(It.IsAny<DogDbModel[]>()), Times.Once);
        }
    }
}
