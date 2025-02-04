﻿using FilePocket.Contracts.Repositories;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.DataAccess.Repositories;

public class PocketRepository : RepositoryBase<Pocket>, IPocketRepository
{
    public PocketRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public async Task<List<Pocket>> GetAllAsync(Guid userId, bool trackChanges)
    {
        return await FindByCondition(c => c.UserId == userId, trackChanges).OrderByDescending(c => c.DateCreated).ToListAsync();
    }

    public async Task<List<Pocket>> GetAllByUserIdAsync(Guid userId, bool trackChanges)
    {
        return await FindByCondition(e => e.UserId.Equals(userId), trackChanges).ToListAsync();
    }

    public async Task<Pocket> GetByIdAsync(Guid userId, Guid pocketId, bool trackChanges)
    {
        return (await FindByCondition(c => c.Id.Equals(pocketId) && c.UserId.Equals(userId), trackChanges).SingleOrDefaultAsync())!;
    }

<<<<<<< Updated upstream
    public async Task<(string Name,string Description, DateTime DateCreated, int NumberOfFiles, double TotalFileSize)> GetPocketDetailsAsync(Guid userId, Guid pocketId, bool trackChanges)
=======
    public async Task<PocketDetailsModel> GetPocketDetailsAsync(Guid userId, Guid pocketId, bool trackChanges)
>>>>>>> Stashed changes
    {
        var query = FindByCondition(c => c.Id.Equals(pocketId) && c.UserId.Equals(userId), trackChanges);

        var pocket = await query
            .Include(s => s.FileMetadata)
            .SingleOrDefaultAsync();

        var totalFileSize = pocket.FileMetadata?.Sum(f => f.FileSize) ?? 0;
        var numberOfFiles = pocket.FileMetadata?.Count ?? 0;
<<<<<<< Updated upstream

        return (pocket.Name,pocket.Description, pocket.DateCreated, numberOfFiles, totalFileSize);
=======
        return new PocketDetailsModel
        {
            Name = pocket.Name,
            Description = pocket.Description,
            DateCreated = pocket.DateCreated,
            NumberOfFiles = numberOfFiles,
            TotalFileSize = totalFileSize
        };
>>>>>>> Stashed changes
    }

    public async Task<double> GetTotalFileSizeAsync(Guid userId, Guid pocketId, bool trackChanges)
    {
        var query = FindByCondition(c => c.Id.Equals(pocketId) && c.UserId.Equals(userId), trackChanges);

        var pocket = await query
            .Include(s => s.FileMetadata)
            .FirstOrDefaultAsync(s => s.Id == pocketId);

        var totalFileSizeInKilobytes = pocket.FileMetadata?.Sum(f => f.FileSize) ?? 0;
        var totalFileSizeInBytes = totalFileSizeInKilobytes * 1024;

        return totalFileSizeInBytes;
    }

    public void CreatePocket(Pocket pocket)
    {
        Create(pocket);
    }

    public void DeletePocket(Pocket pocket)
    {
        Delete(pocket);
    }

    public void UpdatePocket(Pocket pocket)
    {
        Update(pocket);
    }
}
