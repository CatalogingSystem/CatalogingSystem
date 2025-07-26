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

    public CatalogService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
                    join graphicDoc in _context.GraphicDocumentations.AsNoTracking()
                        on archivo.expediente equals graphicDoc.expediente into graphicGroup
                    from graphicDoc in graphicGroup.DefaultIfEmpty()
                    join adminData in _context.AdministrativeData.AsNoTracking()
                        on archivo.expediente equals adminData.FileNumber into adminGroup
                    from adminData in adminGroup.DefaultIfEmpty()
                    join conservation in _context.Conservations.AsNoTracking()
                        on archivo.expediente equals conservation.Expediente into consGroup
                    from conservation in consGroup.DefaultIfEmpty()
                    join dating in _context.Datings.AsNoTracking()
                        on archivo.expediente equals dating.Expediente into datingGroup
                    from dating in datingGroup.DefaultIfEmpty()
                    select new
                    {
                        Archivo = archivo,
                        Identification = identification,
                        GraphicDocumentation = graphicDoc,
                        AdministrativeData = adminData,
                        Conservation = conservation,
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
            GraphicDocumentation = x.GraphicDocumentation != null ? _mapper.Map<GraphicDocumentationDto>(x.GraphicDocumentation) : null,
            AdministrativeData = x.AdministrativeData != null ? _mapper.Map<AdministrativeDataDto>(x.AdministrativeData) : null,
            Conservation = x.Conservation != null ? _mapper.Map<ConservationDto>(x.Conservation) : null,
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
                           join graphicDoc in _context.GraphicDocumentations.AsNoTracking()
                               on archivo.expediente equals graphicDoc.expediente into graphicGroup
                           from graphicDoc in graphicGroup.DefaultIfEmpty()
                           join adminData in _context.AdministrativeData.AsNoTracking()
                               on archivo.expediente equals adminData.FileNumber into adminGroup
                           from adminData in adminGroup.DefaultIfEmpty()
                           join conservation in _context.Conservations.AsNoTracking()
                               on archivo.expediente equals conservation.Expediente into consGroup
                           from conservation in consGroup.DefaultIfEmpty()
                           join dating in _context.Datings.AsNoTracking()
                               on archivo.expediente equals dating.Expediente into datingGroup
                           from dating in datingGroup.DefaultIfEmpty()
                           where archivo.expediente == expediente
                           select new
                           {
                               Archivo = archivo,
                               Identification = identification,
                               GraphicDocumentation = graphicDoc,
                               AdministrativeData = adminData,
                               Conservation = conservation,
                               Dating = dating
                           }).FirstOrDefaultAsync();

        if (result == null || result.Archivo == null) return null;

        return new CatalogItemDto
        {
            Expediente = result.Archivo.expediente,
            ArchivoAdministrativo = _mapper.Map<ArchivoAdministrativoDto>(result.Archivo),
            Identification = result.Identification != null ? _mapper.Map<IdentificationDto>(result.Identification) : null,
            GraphicDocumentation = result.GraphicDocumentation != null ? _mapper.Map<GraphicDocumentationDto>(result.GraphicDocumentation) : null,
            AdministrativeData = result.AdministrativeData != null ? _mapper.Map<AdministrativeDataDto>(result.AdministrativeData) : null,
            Conservation = result.Conservation != null ? _mapper.Map<ConservationDto>(result.Conservation) : null,
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
                    join graphicDoc in _context.GraphicDocumentations.AsNoTracking()
                        on archivo.expediente equals graphicDoc.expediente into graphicGroup
                    from graphicDoc in graphicGroup.DefaultIfEmpty()
                    join adminData in _context.AdministrativeData.AsNoTracking()
                        on archivo.expediente equals adminData.FileNumber into adminGroup
                    from adminData in adminGroup.DefaultIfEmpty()
                    join conservation in _context.Conservations.AsNoTracking()
                        on archivo.expediente equals conservation.Expediente into consGroup
                    from conservation in consGroup.DefaultIfEmpty()
                    join dating in _context.Datings.AsNoTracking()
                        on archivo.expediente equals dating.Expediente into datingGroup
                    from dating in datingGroup.DefaultIfEmpty()
                    select new
                    {
                        Archivo = archivo,
                        Identification = identification,
                        GraphicDocumentation = graphicDoc,
                        AdministrativeData = adminData,
                        Conservation = conservation,
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
            GraphicDocumentation = x.GraphicDocumentation != null ? _mapper.Map<GraphicDocumentationDto>(x.GraphicDocumentation) : null,
            AdministrativeData = x.AdministrativeData != null ? _mapper.Map<AdministrativeDataDto>(x.AdministrativeData) : null,
            Conservation = x.Conservation != null ? _mapper.Map<ConservationDto>(x.Conservation) : null,
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
}