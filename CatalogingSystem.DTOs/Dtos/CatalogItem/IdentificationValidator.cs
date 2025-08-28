using FluentValidation;

namespace CatalogingSystem.DTOs.Dtos;

public class IdentificationValidator : AbstractValidator<IdentificationDto>
{
    public IdentificationValidator()
    {
        RuleFor(x => x.Expediente)
            .GreaterThan(0)
            .WithMessage("Expediente must be a positive number.");

        RuleFor(x => x.Inventory)
            .GreaterThan(0)
            .WithMessage("Inventory must be a positive number.");

        RuleFor(x => x.NumberOfObjects)
            .GreaterThan(0)
            .WithMessage("NumberOfObjects must be a positive number.");

        RuleFor(x => x.GenericClassification)
            .NotEmpty()
            .WithMessage("GenericClassification is required.");

        RuleFor(x => x.ObjectName).NotEmpty().WithMessage("ObjectName is required.");

        RuleFor(x => x.Observations).NotEmpty().WithMessage("Observations is required.");
    }
}
