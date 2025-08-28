using FluentValidation;

namespace CatalogingSystem.DTOs.Dtos;

public class DatingValidator : AbstractValidator<DatingDto>
{
    public DatingValidator()
    {
        RuleFor(x => x.Expediente)
            .GreaterThan(0)
            .WithMessage("Expediente must be a positive number.");
    }
}
