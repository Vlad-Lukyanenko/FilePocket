using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services;

public interface IPocketService
{
    Task<PocketModel> GetByIdAsync(Guid userId, Guid pocketId, bool trackChanges);

    Task<List<PocketModel>> GetAllCustomByUserIdAsync(Guid userId, bool trackChanges);

    Task<PocketModel> GetDefaultByUserIdAsync(Guid userId, bool trackChanges);

    Task<PocketModel> CreatePocketAsync(PocketForManipulationsModel pocket);

    Task UpdatePocketAsync(Guid pocketId, PocketForManipulationsModel pocket, bool trackChanges);

    Task DeletePocketAsync(Guid userId, Guid pocketId, bool trackChanges);

    Task<PocketDetailsModel> GetPocketDetailsAsync(Guid userId, Guid pocketId, bool trackChanges);

    Task<bool> GetComparingDefaultCapacityWithTotalFilesSizeInPocket(Guid userId, Guid pocketId, double newFileSize);

    Task MoveToTrash(Guid userId, Guid pocketId);
}
