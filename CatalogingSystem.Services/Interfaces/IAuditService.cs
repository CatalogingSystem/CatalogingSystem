namespace CatalogingSystem.Services.Interfaces;

public interface IAuditService
{
    Task LogAuditAsync<T>(
        string operation,
        Guid? recordId,
        T? oldData,
        T? newData,
        string? context = null
    );
}
