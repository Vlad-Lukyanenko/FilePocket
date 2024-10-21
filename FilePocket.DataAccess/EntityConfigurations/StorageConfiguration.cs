using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.DataAccess.EntityConfigurations;

public class StorageConfiguration : IEntityTypeConfiguration<Storage>
{
    public void Configure(EntityTypeBuilder<Storage> builder)
    {
        builder.HasMany(s => s.FileUploadSummaries)
            .WithOne(us => us.Storage)
            .HasForeignKey(us => us.StorageId)
            .IsRequired();
    }
}
