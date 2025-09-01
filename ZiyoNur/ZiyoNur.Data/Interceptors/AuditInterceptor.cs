using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;

namespace ZiyoNur.Data.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<AuditInterceptor> _logger;

    public AuditInterceptor(ILogger<AuditInterceptor> logger)
    {
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        LogAuditInformation(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        LogAuditInformation(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void LogAuditInformation(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added ||
                       e.State == EntityState.Modified ||
                       e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var entityName = entry.Entity.GetType().Name;
            var entityId = entry.Entity.Id;
            var operation = entry.State.ToString();

            _logger.LogInformation("Audit: {Operation} {EntityName} with ID {EntityId}",
                operation, entityName, entityId);

            // You can extend this to log specific property changes
            if (entry.State == EntityState.Modified)
            {
                var modifiedProperties = entry.Properties
                    .Where(p => p.IsModified)
                    .Select(p => p.Metadata.Name)
                    .ToList();

                if (modifiedProperties.Any())
                {
                    _logger.LogDebug("Modified properties: {Properties}",
                        string.Join(", ", modifiedProperties));
                }
            }
        }
    }
}
