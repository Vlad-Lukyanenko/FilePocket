using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.DataAccess.EntityConfigurations;

public class FileUploadSummaryConfiguration : IEntityTypeConfiguration<FileUploadSummary>
{
    public void Configure(EntityTypeBuilder<FileUploadSummary> builder)
    {
        builder.HasOne(us => us.Storage)
            .WithMany(s => s.FileUploadSummaries)
            .HasForeignKey(us => us.StorageId)
            .IsRequired();
    }
}
