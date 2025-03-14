using FilePocket.Domain.Entities.Consumption;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.Infrastructure.Persistence.EntityConfigurations.Consumption;

public class AccountConsumptionConfiguration : IEntityTypeConfiguration<AccountConsumption>
{
    private const int DiscriminatorMaxLength = 255;
    private const string DiscriminatorColumnName = nameof(AccountConsumption.MetricType);

    public void Configure(EntityTypeBuilder<AccountConsumption> builder)
    {
        builder.HasKey(ac => ac.Id);

        builder.HasDiscriminator<string>(DiscriminatorColumnName)
            .HasValue<StorageConsumption>(nameof(AccountConsumption.StorageCapacity));

        builder.Property(ac => ac.MetricType)
            .HasMaxLength(DiscriminatorMaxLength)
            .IsRequired();

        builder.Property(ac => ac.UserId)
            .IsRequired();

        builder.HasIndex(ac => new { ac.UserId, ac.MetricType })
            .IsUnique();

        builder.Property(ac => ac.CreatedAt)
            .IsRequired();

        builder.Property(ac => ac.UpdatedAt)
            .IsRequired();

        builder.Property(ac => ac.IsActivated)
            .HasDefaultValue(true);
    }
}