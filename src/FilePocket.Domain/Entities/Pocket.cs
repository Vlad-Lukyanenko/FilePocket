using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Entities;

public class Pocket
{
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public int? NumberOfFiles { get; set; }

    public double? TotalSize { get; set; }

    [Required]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    [MaxLength(500)]
    public string? Description { get; set; }
    public virtual ICollection<FileMetadata>? FileMetadata { get; set; }

    public void AddFile(FileMetadata fileMetadata)
    {
        FileMetadata ??= new List<FileMetadata>();

        if (Contains(fileMetadata))
            throw new ArgumentException($"File already exists in the pocket. File name: {fileMetadata.OriginalName}");

        FileMetadata.Add(fileMetadata);

        NumberOfFiles = NumberOfFiles is null 
            ? 1 
            : NumberOfFiles + 1;

        TotalSize = TotalSize is null 
            ? fileMetadata.FileSize 
            : TotalSize + fileMetadata.FileSize;
    }

    private bool Contains(FileMetadata fileMetadata)
    {
        return FileMetadata?.Any(fm => fm.OriginalName == fileMetadata.OriginalName &&
                                      fm.FileSize.Equals(fileMetadata.FileSize) &&
                                      fm.FileType == fileMetadata.FileType) ?? false;
    }
}
