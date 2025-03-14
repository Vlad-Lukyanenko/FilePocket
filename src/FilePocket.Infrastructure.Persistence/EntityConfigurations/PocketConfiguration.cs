using FilePocket.Domain.Entities;
using FilePocket.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.Infrastructure.Persistence.EntityConfigurations;

public class PocketConfiguration : IEntityTypeConfiguration<Pocket>
{
    private const int MaxNameLength = 255;
    private const int MaxDescriptionLength = 500;

    public void Configure(EntityTypeBuilder<Pocket> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired();

        ConfigureThatPocketNameShouldBeUniqueInUserScope(builder);

        builder.Property(p => p.Name)
            .HasMaxLength(MaxNameLength)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(MaxDescriptionLength)
            .HasDefaultValue(string.Empty)
            .IsRequired(false);

        builder.Property(p => p.NumberOfFiles)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(p => p.TotalSize)
            .HasDefaultPrecision()
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(p => p.IsDefault)
            .HasDefaultValue(false);

        builder.Property(p => p.DateCreated)
            .IsRequired();
    }

    private static void ConfigureThatPocketNameShouldBeUniqueInUserScope(EntityTypeBuilder<Pocket> builder)
    {
        builder.HasIndex(x => new { x.Id, x.UserId, x.Name })
            .IsUnique();
    }
}
