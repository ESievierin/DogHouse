using DogHouseService.BLL.DTO;
using DogHouseService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DogHouseService.API.Controllers
{
    [ApiController]
    public class DogController(IDogService dogService) : ControllerBase
    {
        [HttpGet("dogs")]
        public async Task<DogDto[]> GetAll([FromQuery] DogQueryDto dogQuery, CancellationToken cancellationToken) =>
            await dogService.GetAllAsync(dogQuery, cancellationToken);

        [HttpPost("dog")]
        public async Task CreateDog([FromBody] DogDto dog, CancellationToken cancellationToken) =>
            await dogService.CreateDogAsync(dog, cancellationToken);
    }
}
