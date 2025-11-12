using AutoMapper;
using DogHouseService.BLL.DTO;
using DogHouseService.DAL.DbModels;
using DogHouseService.DAL.EF;
using MediatR;

namespace DogHouseService.BLL.RequestHandlers.Dog
{
    public sealed record CreateDogCommand(DogDto Dog) : IRequest;
    public sealed class CreateDogCommandHandler(DogHouseDbContext dbContext, IMapper mapper) : IRequestHandler<CreateDogCommand>
    {
        public async Task Handle(CreateDogCommand request, CancellationToken cancellationToken) 
        {
            dbContext.Dogs.Add(mapper.Map<DogDbModel>(request.Dog));

            await dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
