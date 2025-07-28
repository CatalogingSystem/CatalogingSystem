using FluentValidation;
namespace CatalogingSystem.DTOs.Dtos;

public class ConservationValidator : AbstractValidator<ConservationDto>
{
    public ConservationValidator()
    {
        RuleFor(x => x.Expediente)
            .GreaterThan(0)
            .WithMessage("Expediente must be a positive number.");
    }
}