using AutoMapper;
using FilePocket.Contracts.Repositories;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using FilePocket.Shared.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FilePocket.Application.Services;

public class StorageService : IStorageService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly double _defaultCapacity;

    public StorageService(IRepositoryManager repository, IMapper mapper, IConfiguration configuration)
    {
        _repository = repository;
        _mapper = mapper;
        _defaultCapacity = configuration.GetValue<double>("DefaultCapacity")!;
    }

    public async Task<IEnumerable<StorageModel>> GetAllAsync(Guid userId, bool trackChanges)
    {
        var storages = await _repository.Storage.GetAllAsync(userId, trackChanges);

        return _mapper.Map<IEnumerable<StorageModel>>(storages);
    }

    public async Task<StorageModel> GetByIdAsync(Guid storageId, bool trackChanges)
    {
        var storage = await GetStorageAndCheckIfItExists(storageId, trackChanges);

        return _mapper.Map<StorageModel>(storage);
    }

    public async Task<StorageDetailsModel> GetStorageDetailsAsync(Guid storageId, bool trackChanges)
    {
        var (name, dateCreated, numberOfFiles, totalFileSize) = await _repository.Storage.GetStorageDetailsAsync(storageId, trackChanges);

        var storage = new StorageDetailsModel
        {
            Name = name,
            DateCreated = dateCreated,
            NumberOfFiles = numberOfFiles,
            TotalFileSize = totalFileSize
        };

        return _mapper.Map<StorageDetailsModel>(storage);
    }

    public async Task<bool> GetComparingDefaultCapacityWithTotalFilesSizeInStorage(Guid storageId, double newFileSize)
    {
        var totalFileSize = await _repository.Storage.GetTotalFileSizeAsync(storageId, trackChanges: false);

        var totalSizeWithNewFile = totalFileSize + newFileSize;

        var defaultCapacityInBytes = _defaultCapacity * 1024 * 1024;

        return totalSizeWithNewFile <= defaultCapacityInBytes;
    }


    public async Task<IEnumerable<StorageModel>> GetAllByUserIdAsync(Guid userId, bool trackChanges)
    {
        var storages = await _repository.Storage.GetAllByUserIdAsync(userId, trackChanges);

        return _mapper.Map<IEnumerable<StorageModel>>(storages);
    }

    public async Task<StorageModel> CreateStorageAsync(StorageForManipulationsModel storage)
    {
        var storageEntity = _mapper.Map<Storage>(storage);

        _repository.Storage.CreateStorage(storageEntity);
        await _repository.SaveChangesAsync();

        return _mapper.Map<StorageModel>(storageEntity);
    }

    public async Task UpdateStorageAsync(Guid storageId, StorageForManipulationsModel storageToUpdate, bool trackChanges)
    {
        var company = await GetStorageAndCheckIfItExists(storageId, trackChanges);

        _mapper.Map(storageToUpdate, company);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteStorageAsync(Guid storageId, bool trackChanges)
    {
        var storageToDelete = await GetStorageAndCheckIfItExists(storageId, trackChanges);

        _repository.Storage.DeleteStorage(storageToDelete);
        await _repository.SaveChangesAsync();
    }

    private async Task<Storage> GetStorageAndCheckIfItExists(Guid id, bool trackChanges)
    {
        var storage = await _repository.Storage.GetByIdAsync(id, trackChanges);

        if (storage is null)
        {
            throw new StorageNotFoundException(id);
        }

        return storage;
    }
}