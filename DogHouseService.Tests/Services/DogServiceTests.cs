using DogHouseService.BLL.DTO;
using DogHouseService.BLL.RequestHandlers.Dog;
using DogHouseService.BLL.Services;
using MediatR;
using Moq;

namespace DogHouseService.Tests.Services;

public class DogServiceTests
{
    private readonly Mock<IMediator> mediator;
    private readonly DogService service;

    public DogServiceTests()
    {
        mediator = new Mock<IMediator>();
        service = new DogService(mediator.Object);
    }

    [Fact]
    public async Task CreateDogAsync_Should_Send_CreateDogCommand()
    {
        var dog = new DogDto
        {
            Name = "Buddy",
            Color = "Brown",
            Tail_length = 10,
            Weight = 25
        };

        mediator
            .Setup(m => m.Send(It.IsAny<CreateDogCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.CreateDogAsync(dog, CancellationToken.None);

        mediator.Verify(m =>
            m.Send(It.Is<CreateDogCommand>(cmd => cmd.Dog == dog),
                   It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_Should_Send_GetAllSortedDogsQuery_And_Return_Result()
    {
        var query = new DogQueryDto
        {
            OrderByMode = BLL.Models.Sorting.OrderByMode.Asc
        };

        var expectedDogs = new[]
        {
            new DogDto { Name = "Buddy", Color = "Brown", Tail_length = 10, Weight = 25 },
            new DogDto { Name = "Charlie", Color = "White", Tail_length = 15, Weight = 30 }
        };

        mediator
            .Setup(m => m.Send(It.IsAny<GetAllSortedDogsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDogs);

        var result = await service.GetAllAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expectedDogs.Length, result.Length);
        Assert.Equal("Buddy", result[0].Name);

        mediator.Verify(m =>
            m.Send(It.Is<GetAllSortedDogsQuery>(q => q.DogQuery == query),
                   It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
