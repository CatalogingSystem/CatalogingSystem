using FluentValidation;

namespace CatalogingSystem.DTOs.Dtos;

public class AdministrativeDataValidator : AbstractValidator<AdministrativeDataDto>
{
    public AdministrativeDataValidator()
    {
        RuleFor(x => x.FileNumber)
            .GreaterThan(0)
            .WithMessage("FileNumber must be a positive number.");
    }
}