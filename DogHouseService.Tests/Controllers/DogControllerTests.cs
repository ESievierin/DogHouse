using DogHouseService.API.Controllers;
using DogHouseService.BLL.DTO;
using DogHouseService.BLL.Interfaces;
using Moq;

namespace DogHouseService.Tests.Controllers
{
    public class DogControllerTests
    {
        private readonly Mock<IDogService> dogServiceMock;
        private readonly DogController controller;

        public DogControllerTests()
        {
            dogServiceMock = new Mock<IDogService>();
            controller = new DogController(dogServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_Should_Call_Service_And_Return_Result()
        {
            var query = new DogQueryDto();
            var expectedDogs = new[]
            {
                new DogDto { Name = "Buddy", Color = "Brown", Tail_length = 10, Weight = 25 },
                new DogDto { Name = "Charlie", Color = "White", Tail_length = 15, Weight = 30 }
            };

            dogServiceMock
                .Setup(s => s.GetAllAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDogs);

            var result = await controller.GetAll(query, CancellationToken.None);

            Assert.Equal(expectedDogs, result);
            dogServiceMock.Verify(s => s.GetAllAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDog_Should_Call_Service()
        {

            var dog = new DogDto { Name = "Buddy", Color = "Brown", Tail_length = 10, Weight = 25 };

            dogServiceMock
                .Setup(s => s.CreateDogAsync(dog, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await controller.CreateDog(dog, CancellationToken.None);

            dogServiceMock.Verify(s => s.CreateDogAsync(dog, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
