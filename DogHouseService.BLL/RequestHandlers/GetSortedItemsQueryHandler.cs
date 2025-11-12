using DogHouseService.BLL.Interfaces.Sorting;
using DogHouseService.BLL.Models.Filters;
using DogHouseService.BLL.Models.Sorting;
using DogHouseService.DAL.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DogHouseService.BLL.RequestHandlers
{
    public record GetSortedItemsQuery<T>(
    Enum? SortAttribute = null,
    FilterPagination? Pagination = null,
    OrderByMode? OrderByMode = null,
    IQueryable<T>? InitialQuery = null) : IRequest<T[]>;
    public sealed class GetSortedItemsQueryHandler<T>(DogHouseDbContext dbContext, ISortStrategyFactory sortStrategyFactory) : IRequestHandler<GetSortedItemsQuery<T>, T[]>
        where T : class
    {
        public async Task<T[]> Handle(GetSortedItemsQuery<T> request, CancellationToken cancellationToken)
        {
            var initialQuery = request.InitialQuery ?? dbContext.Set<T>().AsQueryable();

            var sortStrategy = sortStrategyFactory.GetProvider<T>();

            IQueryable<T> sortQuery = sortStrategy
                .Apply(initialQuery, request.SortAttribute, request.OrderByMode);

            if (request.Pagination is not null)
            {
                sortQuery = sortQuery
                    .Skip(request.Pagination.PageSize * request.Pagination.PageNumber)
                    .Take(request.Pagination.PageSize);
            }

            return await sortQuery.ToArrayAsync(cancellationToken);
        }
    }
}
