using DogHouseService.BLL.DTO;
using FluentValidation;

namespace DogHouseService.BLL.Validators
{
    public sealed class DogQueryValidator : AbstractValidator<DogQueryDto>
    {
        public DogQueryValidator() 
        {
            When(x => x.Pagination is not null, () =>
            {
                RuleFor(x => x.Pagination!.PageNumber)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Page number cannot be negative");

                RuleFor(x => x.Pagination!.PageSize)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage("Page size cannot be lower than 1");
            });
        }
    }
}
