using DogHouseService.BLL.DTO;
using DogHouseService.BLL.Interfaces;
using DogHouseService.BLL.RequestHandlers.Dog;
using MediatR;

namespace DogHouseService.BLL.Services
{
    public sealed class DogService(IMediator mediator) : IDogService
    {
        public async Task CreateDogAsync(DogDto dog, CancellationToken cancellationToken) =>
            await mediator.Send(new CreateDogCommand(dog), cancellationToken);

        public async Task<DogDto[]> GetAllAsync(DogQueryDto dogQuery, CancellationToken cancellationToken) =>
            await mediator.Send(new GetAllSortedDogsQuery(dogQuery), cancellationToken);
    }
}
