using FluentValidation;
namespace CatalogingSystem.DTOs.Dtos;

public class GraphicDocumentationValidator : AbstractValidator<GraphicDocumentationDto>
{
    public GraphicDocumentationValidator()
    {
        RuleFor(x => x.Expediente)
            .GreaterThan(0)
            .WithMessage("Expediente must be a positive number.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.TechnicalData)
            .NotEmpty()
            .WithMessage("TechnicalData is required.");
    }
}