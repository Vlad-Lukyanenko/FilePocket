namespace FilePocket.BlazorClient.Features.Trash
{
    public interface ITrashRequests
    {
        Task MoveFileToTrash(Guid fileId);
        Task MoveFolderToTrash(Guid folderId);
        Task MovePocketToTrash(Guid pocketId);
    }
}
