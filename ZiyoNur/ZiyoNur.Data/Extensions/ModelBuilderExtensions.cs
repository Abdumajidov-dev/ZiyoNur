using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ZiyoNur.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyAllConfigurations(this ModelBuilder modelBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        return modelBuilder;
    }

    public static ModelBuilder ConfigureEnums(this ModelBuilder modelBuilder)
    {
        // This method can be used to configure enum conversions globally if needed
        return modelBuilder;
    }

    public static ModelBuilder ConfigureDateTimeUtc(this ModelBuilder modelBuilder)
    {
        // Configure all DateTime properties to use UTC
        var dateTimeProperties = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

        foreach (var property in dateTimeProperties)
        {
            // This would require a custom value converter for UTC handling
            // property.SetValueConverter(new UtcDateTimeConverter());
        }

        return modelBuilder;
    }
}
