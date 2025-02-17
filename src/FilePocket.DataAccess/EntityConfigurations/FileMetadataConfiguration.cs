using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.DataAccess.EntityConfigurations;

public class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
{
    private const int MaxActualNameLength = 255;
    private const int MaxOriginalNameLength = 255;

    // Not sure, but need to set the max path length to avoid varchar(max) in db level
    public const int MaxPathLength = 255;

    public void Configure(EntityTypeBuilder<FileMetadata> builder)
    {
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
            .IsRequired();

        builder.Property(f => f.UserId)
            .IsRequired();
    }
}
