using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.DataAccess.EntityConfigurations;

public class PocketConfiguration : IEntityTypeConfiguration<Pocket>
{
    public void Configure(EntityTypeBuilder<Pocket> builder)
    {
        builder.Property(p => p.TotalSize)
            .HasColumnType("bigint");
    }
}
