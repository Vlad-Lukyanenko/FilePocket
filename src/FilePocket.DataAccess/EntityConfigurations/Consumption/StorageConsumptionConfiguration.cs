using FilePocket.DataAccess.Extensions;
using FilePocket.Domain.Entities.Consumption;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.DataAccess.EntityConfigurations.Consumption;

public class StorageConsumptionConfiguration : IEntityTypeConfiguration<StorageConsumption>
{
    public void Configure(EntityTypeBuilder<StorageConsumption> builder)
    {
        builder.HasBaseType<AccountConsumption>();

        builder.Property(sc => sc.Used)
            .HasDefaultPrecision()
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(sc => sc.Total)
            .HasDefaultPrecision()
            .IsRequired();
    }
}