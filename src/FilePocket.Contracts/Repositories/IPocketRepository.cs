using FilePocket.Domain.Entities;

namespace FilePocket.Contracts.Repositories;

public interface IPocketRepository
{
    Task<List<Pocket>> GetAllAsync(Guid userId, bool trackChanges);

    Task<Pocket> GetByIdAsync(Guid userId, Guid pocketId, bool trackChanges = false);

    Task<List<Pocket>> GetAllByUserIdAsync(Guid userId, bool trackChanges = false);

    void CreatePocket(Pocket pocket);

    void DeletePocket(Pocket pocket);

    void UpdatePocket(Pocket pocket);

    Task<(string Name,string Description, DateTime DateCreated, int NumberOfFiles, double TotalFileSize)> GetPocketDetailsAsync(Guid userId, Guid pocketId, bool trackChanges);

    Task<double> GetTotalFileSizeAsync(Guid userId, Guid pocketId, bool trackChanges);
}