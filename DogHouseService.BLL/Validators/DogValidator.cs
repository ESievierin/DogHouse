using DogHouseService.BLL.DTO;
using FluentValidation;

namespace DogHouseService.BLL.Validators
{
    public sealed class DogValidator : AbstractValidator<DogDto>
    {
        public DogValidator() 
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("Name cannot be null or empty");

            RuleFor(x => x.Color)
                .NotNull()
                .NotEmpty()
                .WithMessage("Color cannot be null or empty");

            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0)
                .NotNull()
                .WithMessage("Weight cannot be negative or null");

            RuleFor(x => x.Tail_length)
                .GreaterThanOrEqualTo(0)
                .NotNull()
                .WithMessage("Tail length cannot be negative or null");
        }
    }
}
