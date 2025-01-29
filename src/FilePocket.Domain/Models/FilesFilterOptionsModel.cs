namespace FilePocket.Domain.Models
{
    public record FilesFilterOptionsModel(
        Guid UserId,
        Guid? PocketId,
        DateTime? AfterDate,
        DateTime? BeforeDate,
        string OriginalNameContains,
        string FileType,
        int PageSize,
        int PageNumber);
}
