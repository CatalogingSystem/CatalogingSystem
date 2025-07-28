using AutoMapper;
using CatalogingSystem.Data.DbContext;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogingSystem.Services.Implementations;

public class CatalogService : ICatalogService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private const int MaxPageSize = 100;
    private readonly IArchivoAdministrativoService _archivoService;
    private readonly IIdentificationService _identificationService;
    private readonly IDescriptionClassificationService _descriptionService;
    private readonly IAdministrativeDataService _adminDataService;
    private readonly IConservationService _conservationService;
    private readonly IGraphicDocumentationService _graphicDocService;
    private readonly IDatingService _datingService;

    public CatalogService(
        ApplicationDbContext context,
        IMapper mapper,
        IArchivoAdministrativoService archivoService,
        IIdentificationService identificationService,
        IDescriptionClassificationService descriptionService,
        IAdministrativeDataService adminDataService,
        IConservationService conservationService,
        IGraphicDocumentationService graphicDocService,
        IDatingService datingService)
    {
        _context = context;
        _mapper = mapper;
        _archivoService = archivoService;
        _identificationService = identificationService;
        _descriptionService = descriptionService;
        _adminDataService = adminDataService;
        _conservationService = conservationService;
        _graphicDocService = graphicDocService;
        _datingService = datingService;
    }

    public async Task<PagedResultDto<CatalogItemDto>> GetCatalogItems(int page = 1, int size = 10)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 10;
        if (size > MaxPageSize) size = MaxPageSize;

        var query = from archivo in _context.ArchivosAdministrativos.AsNoTracking()
                    join identification in _context.Identifications.AsNoTracking()
                        on archivo.expediente equals identification.expediente into identGroup
                    from identification in identGroup.DefaultIfEmpty()
                    join description in _context.DescriptionClassifications.AsNoTracking()
                        on archivo.expediente equals description.Expediente into descGroup
                    from description in descGroup.DefaultIfEmpty()
                    join adminData in _context.AdministrativeData.AsNoTracking()
                        on archivo.expediente equals adminData.FileNumber into adminGroup
                    from adminData in adminGroup.DefaultIfEmpty()
                    join conservation in _context.Conservations.AsNoTracking()
                        on archivo.expediente equals conservation.Expediente into consGroup
                    from conservation in consGroup.DefaultIfEmpty()
                    join graphicDoc in _context.GraphicDocumentations.AsNoTracking()
                        on archivo.expediente equals graphicDoc.expediente into graphicGroup
                    from graphicDoc in graphicGroup.DefaultIfEmpty()
                    join dating in _context.Datings.AsNoTracking()
                        on archivo.expediente equals dating.Expediente into datingGroup
                    from dating in datingGroup.DefaultIfEmpty()
                    select new
                    {
                        Archivo = archivo,
                        Identification = identification,
                        DescriptionClassification = description, 
                        AdministrativeData = adminData,
                        Conservation = conservation,
                        GraphicDocumentation = graphicDoc,
                        Dating = dating
                    };

        int totalItems = await query.CountAsync();

        var results = await query
            .OrderBy(x => x.Archivo.expediente)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var catalogItems = results.Select(x => new CatalogItemDto
        {
            Expediente = x.Archivo.expediente,
            ArchivoAdministrativo = _mapper.Map<ArchivoAdministrativoDto>(x.Archivo),
            Identification = x.Identification != null ? _mapper.Map<IdentificationDto>(x.Identification) : null,
            DescriptionClassification = x.DescriptionClassification != null ? _mapper.Map<DescriptionClassificationDto>(x.DescriptionClassification) : null,
            AdministrativeData = x.AdministrativeData != null ? _mapper.Map<AdministrativeDataDto>(x.AdministrativeData) : null,
            Conservation = x.Conservation != null ? _mapper.Map<ConservationDto>(x.Conservation) : null,
            GraphicDocumentation = x.GraphicDocumentation != null ? _mapper.Map<GraphicDocumentationDto>(x.GraphicDocumentation) : null,
            Dating = x.Dating != null ? _mapper.Map<DatingDto>(x.Dating) : null
        }).ToList();

        return new PagedResultDto<CatalogItemDto>
        {
            Items = catalogItems,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)size),
            CurrentPage = page,
            PageSize = size
        };
    }

    public async Task<CatalogItemDto?> GetCatalogItem(long expediente)
    {
        var result = await (from archivo in _context.ArchivosAdministrativos.AsNoTracking()
                        join identification in _context.Identifications.AsNoTracking()
                            on archivo.expediente equals identification.expediente into identGroup
                        from identification in identGroup.DefaultIfEmpty()
                        join description in _context.DescriptionClassifications.AsNoTracking()
                            on archivo.expediente equals description.Expediente into descGroup
                        from description in descGroup.DefaultIfEmpty()
                        join adminData in _context.AdministrativeData.AsNoTracking()
                            on archivo.expediente equals adminData.FileNumber into adminGroup
                        from adminData in adminGroup.DefaultIfEmpty()
                        join conservation in _context.Conservations.AsNoTracking()
                            on archivo.expediente equals conservation.Expediente into consGroup
                        from conservation in consGroup.DefaultIfEmpty()
                        join graphicDoc in _context.GraphicDocumentations.AsNoTracking()
                            on archivo.expediente equals graphicDoc.expediente into graphicGroup
                        from graphicDoc in graphicGroup.DefaultIfEmpty()
                        join dating in _context.Datings.AsNoTracking()
                            on archivo.expediente equals dating.Expediente into datingGroup
                        from dating in datingGroup.DefaultIfEmpty()
                        where archivo.expediente == expediente
                        select new
                        {
                            Archivo = archivo,
                            Identification = identification,
                            DescriptionClassification = description,
                            AdministrativeData = adminData,
                            Conservation = conservation,
                            GraphicDocumentation = graphicDoc,
                            Dating = dating
                        }).FirstOrDefaultAsync();

        if (result == null || result.Archivo == null) return null;

        return new CatalogItemDto
        {
            Expediente = result.Archivo.expediente,
            ArchivoAdministrativo = _mapper.Map<ArchivoAdministrativoDto>(result.Archivo),
            Identification = result.Identification != null ? _mapper.Map<IdentificationDto>(result.Identification) : null,
            DescriptionClassification = result.DescriptionClassification != null ? _mapper.Map<DescriptionClassificationDto>(result.DescriptionClassification) : null,
            AdministrativeData = result.AdministrativeData != null ? _mapper.Map<AdministrativeDataDto>(result.AdministrativeData) : null,
            Conservation = result.Conservation != null ? _mapper.Map<ConservationDto>(result.Conservation) : null,
            GraphicDocumentation = result.GraphicDocumentation != null ? _mapper.Map<GraphicDocumentationDto>(result.GraphicDocumentation) : null,
            Dating = result.Dating != null ? _mapper.Map<DatingDto>(result.Dating) : null
        };
    }

    public async Task<PagedResultDto<CatalogItemDto>> SearchCatalogItems(
        string? materialName,
        string? authorName,
        string? titleName,
        string? genericClassification,
        int page = 1,
        int size = 10)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 10;
        if (size > MaxPageSize) size = MaxPageSize;

        var query = from archivo in _context.ArchivosAdministrativos.AsNoTracking()
                    join identification in _context.Identifications.AsNoTracking()
                        on archivo.expediente equals identification.expediente into identGroup
                    from identification in identGroup.DefaultIfEmpty()
                    join description in _context.DescriptionClassifications.AsNoTracking()
                        on archivo.expediente equals description.Expediente into descGroup
                    from description in descGroup.DefaultIfEmpty()
                    join adminData in _context.AdministrativeData.AsNoTracking()
                        on archivo.expediente equals adminData.FileNumber into adminGroup
                    from adminData in adminGroup.DefaultIfEmpty()
                    join conservation in _context.Conservations.AsNoTracking()
                        on archivo.expediente equals conservation.Expediente into consGroup
                    from conservation in consGroup.DefaultIfEmpty()
                    join graphicDoc in _context.GraphicDocumentations.AsNoTracking()
                        on archivo.expediente equals graphicDoc.expediente into graphicGroup
                    from graphicDoc in graphicGroup.DefaultIfEmpty()
                    join dating in _context.Datings.AsNoTracking()
                        on archivo.expediente equals dating.Expediente into datingGroup
                    from dating in datingGroup.DefaultIfEmpty()
                    select new
                    {
                        Archivo = archivo,
                        Identification = identification,
                        DescriptionClassification = description,
                        AdministrativeData = adminData,
                        Conservation = conservation,
                        GraphicDocumentation = graphicDoc,
                        Dating = dating
                    };

        if (!string.IsNullOrEmpty(materialName) || !string.IsNullOrEmpty(authorName) ||
            !string.IsNullOrEmpty(titleName) || !string.IsNullOrEmpty(genericClassification))
        {
            query = query.Where(x => x.Identification != null &&
                (string.IsNullOrEmpty(materialName) || EF.Functions.ILike(x.Identification.material.MaterialName, $"%{materialName}%")) &&
                (string.IsNullOrEmpty(authorName) || EF.Functions.ILike(x.Identification.author.Name, $"%{authorName}%")) &&
                (string.IsNullOrEmpty(titleName) || EF.Functions.ILike(x.Identification.title.Name, $"%{titleName}%")) &&
                (string.IsNullOrEmpty(genericClassification) || EF.Functions.ILike(x.Identification.genericClassification, $"%{genericClassification}%")));
        }

        int totalItems = await query.CountAsync();

        var results = await query
            .OrderBy(x => x.Archivo.expediente)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var catalogItems = results.Select(x => new CatalogItemDto
        {
            Expediente = x.Archivo.expediente,
            ArchivoAdministrativo = _mapper.Map<ArchivoAdministrativoDto>(x.Archivo),
            Identification = x.Identification != null ? _mapper.Map<IdentificationDto>(x.Identification) : null,
            DescriptionClassification = x.DescriptionClassification != null ? _mapper.Map<DescriptionClassificationDto>(x.DescriptionClassification) : null,
            AdministrativeData = x.AdministrativeData != null ? _mapper.Map<AdministrativeDataDto>(x.AdministrativeData) : null,
            Conservation = x.Conservation != null ? _mapper.Map<ConservationDto>(x.Conservation) : null,
            GraphicDocumentation = x.GraphicDocumentation != null ? _mapper.Map<GraphicDocumentationDto>(x.GraphicDocumentation) : null,
            Dating = x.Dating != null ? _mapper.Map<DatingDto>(x.Dating) : null
        }).ToList();

        return new PagedResultDto<CatalogItemDto>
        {
            Items = catalogItems,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)size),
            CurrentPage = page,
            PageSize = size
        };
    }

    public async Task<bool> DeleteCatalogItem(long expediente)
    {
        var archivo = await _context.ArchivosAdministrativos
            .FirstOrDefaultAsync(a => a.expediente == expediente);
        if (archivo == null) return false;

        // Eliminar todos los TemporalMovement asociados al expediente
        var temporalMovements = await _context.TemporalMovements
            .Where(tm => tm.Expediente == expediente)
            .ToListAsync();
        if (temporalMovements.Any())
        {
            _context.TemporalMovements.RemoveRange(temporalMovements);
        }

        var graphicDoc = await _context.GraphicDocumentations
            .FirstOrDefaultAsync(g => g.expediente == expediente);
        if (graphicDoc != null)
        {
            _context.GraphicDocumentations.Remove(graphicDoc);
        }

        var identification = await _context.Identifications
            .FirstOrDefaultAsync(i => i.expediente == expediente);
        if (identification != null)
        {
            _context.Identifications.Remove(identification);
        }
        
        var description = await _context.DescriptionClassifications 
            .FirstOrDefaultAsync(d => d.Expediente == expediente);
        if (description != null)
        {
            _context.DescriptionClassifications.Remove(description);
        }

        var adminData = await _context.AdministrativeData
            .FirstOrDefaultAsync(ad => ad.FileNumber == expediente);
        if (adminData != null)
        {
            _context.AdministrativeData.Remove(adminData);
        }

        var conservation = await _context.Conservations
            .FirstOrDefaultAsync(c => c.Expediente == expediente);
        if (conservation != null)
        {
            _context.Conservations.Remove(conservation);
        }

        var dating = await _context.Datings
            .FirstOrDefaultAsync(d => d.Expediente == expediente);
        if (dating != null)
        {
            _context.Datings.Remove(dating);
        }

        _context.ArchivosAdministrativos.Remove(archivo);

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<CatalogItemDto?> ExportCatalogItem(long expediente)
    {
        var result = await (from archivo in _context.ArchivosAdministrativos.AsNoTracking()
                            join identification in _context.Identifications.AsNoTracking()
                                on archivo.expediente equals identification.expediente into identGroup
                            from identification in identGroup.DefaultIfEmpty()
                            join description in _context.DescriptionClassifications.AsNoTracking()
                                on archivo.expediente equals description.Expediente into descGroup
                            from description in descGroup.DefaultIfEmpty()
                            join adminData in _context.AdministrativeData.AsNoTracking()
                                on archivo.expediente equals adminData.FileNumber into adminGroup
                            from adminData in adminGroup.DefaultIfEmpty()
                            join conservation in _context.Conservations.AsNoTracking()
                                on archivo.expediente equals conservation.Expediente into consGroup
                            from conservation in consGroup.DefaultIfEmpty()
                            join graphicDoc in _context.GraphicDocumentations.AsNoTracking()
                                on archivo.expediente equals graphicDoc.expediente into graphicGroup
                            from graphicDoc in graphicGroup.DefaultIfEmpty()
                            join dating in _context.Datings.AsNoTracking()
                                on archivo.expediente equals dating.Expediente into datingGroup
                            from dating in datingGroup.DefaultIfEmpty()
                            where archivo.expediente == expediente
                            select new
                            {
                                Archivo = archivo,
                                Identification = identification,
                                DescriptionClassification = description,
                                AdministrativeData = adminData,
                                Conservation = conservation,
                                GraphicDocumentation = graphicDoc,
                                Dating = dating
                            }).FirstOrDefaultAsync();

        if (result == null || result.Archivo == null) return null;

        return new CatalogItemDto
        {
            Expediente = result.Archivo.expediente,
            ArchivoAdministrativo = _mapper.Map<ArchivoAdministrativoDto>(result.Archivo),
            Identification = result.Identification != null ? _mapper.Map<IdentificationDto>(result.Identification) : null,
            DescriptionClassification = result.DescriptionClassification != null ? _mapper.Map<DescriptionClassificationDto>(result.DescriptionClassification) : null,
            AdministrativeData = result.AdministrativeData != null ? _mapper.Map<AdministrativeDataDto>(result.AdministrativeData) : null,
            Conservation = result.Conservation != null ? _mapper.Map<ConservationDto>(result.Conservation) : null,
            GraphicDocumentation = result.GraphicDocumentation != null ? _mapper.Map<GraphicDocumentationDto>(result.GraphicDocumentation) : null,
            Dating = result.Dating != null ? _mapper.Map<DatingDto>(result.Dating) : null
        };
    }

    public async Task ImportCatalogItems(List<CatalogItemDto> catalogItems, long? nuevoExpediente = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var duplicateExpedientes = catalogItems
                .GroupBy(x => x.Expediente)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateExpedientes.Any())
            {
                throw new InvalidOperationException($"Expedientes duplicados encontrados en el archivo: {string.Join(", ", duplicateExpedientes)}");
            }

            foreach (var item in catalogItems)
            {
                long efectivoExpediente = nuevoExpediente.HasValue ? nuevoExpediente.Value : item.Expediente;

                if (!nuevoExpediente.HasValue)
                {
                    var existingArchivo = await _context.ArchivosAdministrativos
                        .FirstOrDefaultAsync(a => a.expediente == item.Expediente);
                    if (existingArchivo != null)
                    {
                        throw new InvalidOperationException($"El expediente {item.Expediente} ya existe.");
                    }
                }

                item.Expediente = efectivoExpediente;
                if (item.ArchivoAdministrativo != null) item.ArchivoAdministrativo.Expediente = efectivoExpediente;
                if (item.Identification != null) item.Identification.Expediente = efectivoExpediente;
                if (item.DescriptionClassification != null) item.DescriptionClassification.Expediente = efectivoExpediente;
                if (item.AdministrativeData != null) item.AdministrativeData.FileNumber = efectivoExpediente;
                if (item.Conservation != null) item.Conservation.Expediente = efectivoExpediente;
                if (item.GraphicDocumentation != null) item.GraphicDocumentation.Expediente = efectivoExpediente;
                if (item.Dating != null) item.Dating.Expediente = efectivoExpediente;

                var archivo = await _archivoService.CreateArchivoAdministrativo(item.ArchivoAdministrativo);

                if (item.Identification != null)
                {
                    await _identificationService.CreateIdentification(item.Identification);
                }

                if (item.DescriptionClassification != null)
                {
                    await _descriptionService.CreateDescriptionClassification(item.DescriptionClassification);
                }

                if (item.AdministrativeData != null)
                {
                    await _adminDataService.CreateAdministrativeData(item.AdministrativeData);
                }

                if (item.Conservation != null)
                {
                    await _conservationService.CreateConservation(item.Conservation);
                }

                if (item.GraphicDocumentation != null)
                {
                    await _graphicDocService.CreateGraphicDocumentation(item.GraphicDocumentation);
                }

                if (item.Dating != null)
                {
                    await _datingService.CreateDating(item.Dating);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}