namespace FilePocket.Shared.Exceptions;

public class FileUploadSummaryNotFoundException : NotFoundException
{
    public FileUploadSummaryNotFoundException(Guid fileUploadSummaryId)
        : base($"The FileUploadSummary with id '{fileUploadSummaryId}' doesn't exist in the database.")
    {
    }
}
