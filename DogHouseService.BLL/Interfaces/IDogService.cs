using DogHouseService.BLL.DTO;

namespace DogHouseService.BLL.Interfaces
{
    public interface IDogService
    {
        public Task<DogDto[]> GetAllAsync(DogQueryDto dogQuery, CancellationToken cancellationToken);
        public Task CreateDogAsync(DogDto dog,CancellationToken cancellationToken);
    }
}
