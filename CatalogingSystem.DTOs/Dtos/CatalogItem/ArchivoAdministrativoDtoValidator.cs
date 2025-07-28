using FluentValidation;
namespace CatalogingSystem.DTOs.Dtos;
public class ArchivoAdministrativoDtoValidator : AbstractValidator<ArchivoAdministrativoDto>
{
    public ArchivoAdministrativoDtoValidator()
    {
        RuleFor(x => x.Institucion)
            .IsInEnum()
            .WithMessage("Institucion debe ser un valor válido de TipoInstitucion.");
        RuleFor(x => x.Unidad).NotEmpty().WithMessage("Unidad es requerida.");
        RuleFor(x => x.Expediente).GreaterThan(0).WithMessage("Expediente debe ser mayor que cero.");
        RuleFor(x => x.DocumentoOrigen)
            .IsInEnum()
            .WithMessage("DocumentoOrigen debe ser un valor válido de TipoDocumentoOrigen.");
        RuleFor(x => x.FechaInicial).NotEmpty().WithMessage("FechaInicial es requerida.");
    }
}