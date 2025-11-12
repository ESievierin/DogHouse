using DogHouseService.BLL.Models.Filters;
using DogHouseService.BLL.Models.Sorting;

namespace DogHouseService.BLL.DTO
{
    public class DogQueryDto
    {
        public FilterPagination? Pagination { get; set;}
        public DogsOrderAttribute? OrderAttribute { get; set; }
        public OrderByMode? OrderByMode { get; set; }
    }
}
