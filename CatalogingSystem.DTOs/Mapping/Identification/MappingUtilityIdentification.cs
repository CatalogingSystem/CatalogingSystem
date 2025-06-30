namespace CatalogingSystem.DTOs.Mapping;

using CatalogingSystem.Core.Entities;
using CatalogingSystem.DTOs.Dtos;

public static class MappingUtilityIdentification
{
    public static void MapUpdateIdentificationDtoToEntity(UpdateIdentificationDto dto, Identification identification)
    {
        identification.inventory = dto.Inventory;
        identification.numberOfObjects = dto.NumberOfObjects;
        identification.genericClassification = dto.GenericClassification;
        identification.objectName = dto.ObjectName;
        identification.observations = dto.Observations;

        if (identification.section != null && dto.Section != null)
        {
            identification.section.Room = dto.Section.Room;
            identification.section.Panel = dto.Section.Panel;
            identification.section.DisplayCase = dto.Section.DisplayCase;
            identification.section.Easel = dto.Section.Easel;
            identification.section.Storage = dto.Section.Storage;
            identification.section.Courtyard = dto.Section.Courtyard;
            identification.section.Pillar = dto.Section.Pillar;
            identification.section.Others = dto.Section.Others;
        }

        if (identification.typology != null && dto.Typology != null)
        {
            identification.typology.Type = dto.Typology.Type;
            identification.typology.Subtype = dto.Typology.Subtype;
            identification.typology.Class = dto.Typology.Class;
            identification.typology.Subclass = dto.Typology.Subclass;
            identification.typology.Order = dto.Typology.Order;
            identification.typology.Suborder = dto.Typology.Suborder;
        }

        if (identification.specificName != null && dto.SpecificName != null)
        {
            identification.specificName.GenericName = dto.SpecificName.GenericName;
            identification.specificName.RelatedTerms = dto.SpecificName.RelatedTerms;
            identification.specificName.SpecificTerms = dto.SpecificName.SpecificTerms;
            identification.specificName.UsedBy = dto.SpecificName.UsedBy;
            identification.specificName.Notes = dto.SpecificName.Notes;
        }

        if (identification.author != null && dto.Author != null)
        {
            identification.author.Name = dto.Author.Name;
            identification.author.BirthPlace = dto.Author.BirthPlace;
            identification.author.BirthDate = dto.Author.BirthDate;
            identification.author.DeathPlace = dto.Author.DeathPlace;
            identification.author.DeathDate = dto.Author.DeathDate;
        }

        if (identification.title != null && dto.Title != null)
        {
            identification.title.Name = dto.Title.Name;
            identification.title.Attribution = dto.Title.Attribution;
            identification.title.Translation = dto.Title.Translation;
        }

        if (identification.material != null && dto.Material != null)
        {
            identification.material.DescribedPart = dto.Material.DescribedPart;
            identification.material.MaterialName = dto.Material.MaterialName;
            identification.material.Colors = dto.Material.Colors;
        }

        if (identification.techniques != null && dto.Techniques != null)
        {
            identification.techniques.DescribedPart = dto.Techniques.DescribedPart;
            identification.techniques.TechniqueName = dto.Techniques.TechniqueName;
        }
    }
}