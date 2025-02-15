using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.Contracts.Repositories;

public interface IPocketRepository
{
    Task<List<Pocket>> GetAllAsync(Guid userId, bool trackChanges);

    Task<Pocket> GetByIdAsync(Guid userId, Guid pocketId, bool trackChanges = false);
    
    Task<Pocket> GetDefaultAsync(Guid userId, bool trackChanges = false);

    Task<List<Pocket>> GetAllCustomByUserIdAsync(Guid userId, bool trackChanges = false);

    void CreatePocket(Pocket pocket);

    void DeletePocket(Pocket pocket);

    void UpdatePocket(Pocket pocket);

    Task<PocketDetailsModel> GetPocketDetailsAsync(Guid userId, Guid pocketId, bool trackChanges);
    Task<double> GetTotalFileSizeAsync(Guid userId, Guid pocketId, bool trackChanges);
}