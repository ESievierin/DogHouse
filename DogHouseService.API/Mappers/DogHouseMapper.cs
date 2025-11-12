using AutoMapper;
using DogHouseService.BLL.DTO;
using DogHouseService.DAL.DbModels;

namespace DogHouseService.API.Mappers
{
    public sealed class DogHouseMapper : Profile
    {
        public DogHouseMapper()
        {
            CreateMap<DogDto, DogDbModel>().ReverseMap();
        }
    }
}
