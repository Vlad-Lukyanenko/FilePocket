namespace FilePocket.Client.Features.Trash
{
    public class TrashRequests : ITrashRequests
    {
        private readonly FilePocketApiClient _apiClient;

        public TrashRequests(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task MoveFileToTrash(Guid fileId)
        {
            var _ = await _apiClient.PutAsync($"api/trash/files/{fileId}", null);
        }

        public async Task MoveFolderToTrash(Guid folderId)
        {
            var _ = await _apiClient.PutAsync($"api/trash/folders/{folderId}", null);
        }

        public async Task MovePocketToTrash(Guid pocketId)
        {
            var _ = await _apiClient.PutAsync($"api/trash/pockets/{pocketId}", null);
        }
    }
}
