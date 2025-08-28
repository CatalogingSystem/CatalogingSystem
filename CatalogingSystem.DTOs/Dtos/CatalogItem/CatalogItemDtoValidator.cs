using FluentValidation;

namespace CatalogingSystem.DTOs.Dtos;

public class CatalogItemDtoValidator : AbstractValidator<CatalogItemDto>
{
    public CatalogItemDtoValidator()
    {
        RuleFor(x => x.Expediente)
            .GreaterThan(0)
            .WithMessage("El expediente debe ser mayor que cero.");

        RuleFor(x => x.ArchivoAdministrativo)
            .NotNull()
            .WithMessage("ArchivoAdministrativo es requerido.")
            .SetValidator(new ArchivoAdministrativoDtoValidator());

        RuleFor(x => x.Identification)
            .SetValidator(new IdentificationValidator())
            .When(x => x.Identification != null);

        RuleFor(x => x.DescriptionClassification)
            .SetValidator(new DescriptionClassificationValidator())
            .When(x => x.DescriptionClassification != null);

        RuleFor(x => x.AdministrativeData)
            .SetValidator(new AdministrativeDataValidator())
            .When(x => x.AdministrativeData != null);

        RuleFor(x => x.Conservation)
            .SetValidator(new ConservationValidator())
            .When(x => x.Conservation != null);

        RuleFor(x => x.GraphicDocumentation)
            .SetValidator(new GraphicDocumentationValidator())
            .When(x => x.GraphicDocumentation != null);

        RuleFor(x => x.Dating).SetValidator(new DatingValidator()).When(x => x.Dating != null);
    }
}
