using FilePocket.Domain.Entities;
using FilePocket.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.Infrastructure.Persistence.EntityConfigurations;

public class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
{
    private const int MaxActualNameLength = 255;
    private const int MaxOriginalNameLength = 255;

    public void Configure(EntityTypeBuilder<FileMetadata> builder)
    {
        builder.HasOne(fm => fm.Folder)
              .WithMany(f => f.FileMetadata)
              .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.UserId)
            .IsRequired();

        builder.HasIndex(f => f.ActualName)
            .IsUnique();

        builder.Property(f => f.ActualName)
            .HasMaxLength(MaxActualNameLength)
            .IsRequired();

        builder.Property(f => f.OriginalName)
            .HasMaxLength(MaxOriginalNameLength)
            .IsRequired();

        builder.Property(f => f.Path)
            .IsRequired();

        builder.Property(f => f.FileType)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.FileSize)
            .HasDefaultPrecision()
            .HasDefaultValue(0)
            .IsRequired();
    }
}
