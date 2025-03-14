using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.Infrastructure.Persistence.Extensions;

internal static class PropertyBuilderExtensions
{
    private const int DefaultPrecision = 18;
    private const int DefaultScale = 2;
    
    public static PropertyBuilder<double> HasDefaultPrecision(
        this PropertyBuilder<double> propertyBuilder, 
        int precision = DefaultPrecision,
        int scale = DefaultScale)
    {
        return propertyBuilder.HasPrecision(precision, scale);
    }
}