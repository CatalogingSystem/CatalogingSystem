using FluentValidation;
namespace CatalogingSystem.DTOs.Dtos;

public class DescriptionClassificationValidator : AbstractValidator<DescriptionClassificationDto>
{
    public DescriptionClassificationValidator()
    {
        RuleFor(x => x.Expediente)
            .GreaterThan(0)
            .WithMessage("Expediente must be a positive number.");
    }
}