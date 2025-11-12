using AutoMapper;
using DogHouseService.BLL.DTO;
using DogHouseService.DAL.DbModels;
using MediatR;

namespace DogHouseService.BLL.RequestHandlers.Dog
{
    public record GetAllSortedDogsQuery(DogQueryDto DogQuery) : IRequest<DogDto[]>;
    public sealed class GetAllSortedDogsQueryHandler(IMapper mapper, IMediator mediator) : IRequestHandler<GetAllSortedDogsQuery, DogDto[]>
    {
        public async Task<DogDto[]> Handle(GetAllSortedDogsQuery request, CancellationToken cancellationToken)
        {
            var sortedDogs = await mediator.Send(new GetSortedItemsQuery<DogDbModel>(request.DogQuery.OrderAttribute, request.DogQuery.Pagination, request.DogQuery.OrderByMode), cancellationToken);
            return mapper.Map<DogDto[]>(sortedDogs);
        }
    }
}
