using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilePocket.Infrastructure.Persistence.EntityConfigurations;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.HasOne(b => b.Folder)
               .WithMany(f => f.Bookmarks)
               .OnDelete(DeleteBehavior.Cascade);
    }
}