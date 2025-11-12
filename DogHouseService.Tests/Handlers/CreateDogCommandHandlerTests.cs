using AutoMapper;
using DogHouseService.BLL.DTO;
using DogHouseService.BLL.RequestHandlers.Dog;
using DogHouseService.DAL.DbModels;
using DogHouseService.DAL.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DogHouseService.Tests.RequestHandlers.Dog
{
    public class CreateDogCommandHandlerTests
    {
        private readonly DogHouseDbContext _dbContext;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateDogCommandHandler _handler;

        public CreateDogCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DogHouseDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new DogHouseDbContext(options);
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateDogCommandHandler(_dbContext, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Add_Dog_When_Not_Exist()
        {
            var dogDto = new DogDto
            {
                Name = "Buddy",
                Color = "Brown",
                Tail_length = 10,
                Weight = 25
            };

            var dbModel = new DogDbModel
            {
                Name = dogDto.Name,
                Color = dogDto.Color,
                Tail_length = dogDto.Tail_length,
                Weight = dogDto.Weight
            };

            _mapperMock
                .Setup(m => m.Map<DogDbModel>(dogDto))
                .Returns(dbModel);

            await _handler.Handle(new CreateDogCommand(dogDto), CancellationToken.None);

            var addedDog = await _dbContext.Dogs.FirstOrDefaultAsync(d => d.Name == "Buddy");
            Assert.NotNull(addedDog);
            Assert.Equal("Brown", addedDog.Color);
        }
    }
}
