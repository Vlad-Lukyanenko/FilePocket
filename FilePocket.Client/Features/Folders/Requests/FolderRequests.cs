using FilePocket.Client.Services.Pockets.Models;

namespace FilePocket.Client.Services.Folders.Requests
{
    public class FolderRequests : IFolderRequests
    {
        public Task<IEnumerable<PocketModel>> GetAllAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(CreatePocketModel pocket)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(PocketModel pocket)
        {
            throw new NotImplementedException();
        }
    }
}
