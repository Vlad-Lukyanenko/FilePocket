using AutoMapper;
using FilePocket.Contracts.Repositories;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using FilePocket.Shared.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FilePocket.Application.Services;

public class PocketService : IPocketService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly double _defaultCapacity;

    public PocketService(IRepositoryManager repository, IMapper mapper, IConfiguration configuration)
    {
        _repository = repository;
        _mapper = mapper;
        _defaultCapacity = configuration.GetValue<double>("DefaultCapacity")!;
    }

    public async Task<PocketModel> GetByIdAsync(Guid userId, Guid pocketId, bool trackChanges)
    {
        var pocket = await GetPocketAndCheckIfItExists(userId, pocketId, trackChanges);

        return _mapper.Map<PocketModel>(pocket);
    }

    public async Task<PocketDetailsModel> GetPocketDetailsAsync(Guid userId, Guid pocketId, bool trackChanges)
    {
<<<<<<< Updated upstream
        var (name, description, dateCreated, numberOfFiles, totalFileSize) = await _repository.Pocket.GetPocketDetailsAsync(userId, pocketId, trackChanges);

        var pocket = new PocketDetailsModel
        {
            Name = name,
            Description = description,
            DateCreated = dateCreated,
            NumberOfFiles = numberOfFiles,
            TotalFileSize = totalFileSize
        };

        return _mapper.Map<PocketDetailsModel>(pocket);
=======
        return await _repository.Pocket.GetPocketDetailsAsync(userId, pocketId, trackChanges);
>>>>>>> Stashed changes
    }

    public async Task<bool> GetComparingDefaultCapacityWithTotalFilesSizeInPocket(Guid userId, Guid pocketId, double newFileSize)
    {
        var totalFileSize = await _repository.Pocket.GetTotalFileSizeAsync(userId, pocketId, trackChanges: false);

        var totalSizeWithNewFile = totalFileSize + newFileSize;

        var defaultCapacityInBytes = _defaultCapacity * 1024 * 1024;

        return totalSizeWithNewFile <= defaultCapacityInBytes;
    }

    public async Task<List<PocketModel>> GetAllByUserIdAsync(Guid userId, bool trackChanges)
    {
        var pockets = await _repository.Pocket.GetAllByUserIdAsync(userId, trackChanges);

        return _mapper.Map<List<PocketModel>>(pockets);
    }

    public async Task<PocketModel> CreatePocketAsync(PocketForManipulationsModel pocket)
    {
        var pocketEntity = _mapper.Map<Pocket>(pocket);

        _repository.Pocket.CreatePocket(pocketEntity);
        await _repository.SaveChangesAsync();

        return _mapper.Map<PocketModel>(pocketEntity);
    }

    public async Task UpdatePocketAsync(Guid pocketId, PocketForManipulationsModel pocketToUpdate, bool trackChanges)
    {
        var pocket = await GetPocketAndCheckIfItExists(pocketToUpdate.UserId, pocketId, trackChanges);

        _mapper.Map(pocketToUpdate, pocket);
        await _repository.SaveChangesAsync();
    }

    public async Task DeletePocketAsync(Guid userId, Guid pocketId, bool trackChanges)
    {
        var pocketToDelete = await GetPocketAndCheckIfItExists(userId, pocketId, trackChanges);

        _repository.Pocket.DeletePocket(pocketToDelete);
        await _repository.SaveChangesAsync();
    }

    private async Task<Pocket> GetPocketAndCheckIfItExists(Guid userId, Guid pocketId, bool trackChanges)
    {
        var pocket = await _repository.Pocket.GetByIdAsync(userId, pocketId, trackChanges);

        if (pocket is null)
        {
            throw new PocketNotFoundException(pocketId);
        }

        return pocket;
    }
}