using AutoMapper;
using FilePocket.Contracts.Repositories;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.Application.Services
{
    public class SharedFileService : ISharedFileService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public SharedFileService(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task CreateAsync(Guid userId, SharedFileModel sharedFile)
        {
            var file = _mapper.Map<SharedFile>(sharedFile);

            file.UserId = userId;

            _repository.SharedFile.Create(file);
            await _repository.SaveChangesAsync();
        }

        public async Task<SharedFileModel?> GetByIdAsync(Guid sharedFileId)
        {
            var response = await _repository.SharedFile.GetAggregatedDataByIdAsync(sharedFileId);

            return _mapper.Map<SharedFileModel?>(response);
        }
        public async Task<List<SharedFileView>> GetAllAsync(Guid userId, bool trackChanges)
        {
            var response = await _repository.SharedFile.GetAllAsync(userId, trackChanges);

            return response;
        }

        public async Task Delete(Guid sharedFileId)
        {
            var sharedFile = await _repository.SharedFile.GetByIdAsync(sharedFileId);

            if (sharedFile is not null)
            {
                 _repository.SharedFile.Delete(sharedFile);
                 await _repository.SaveChangesAsync();
            }
        }

        public async Task<byte[]?> DownloadFileAsync(Guid sharedFileId)
        {
            var sharedFileBody = await _repository.SharedFile.GetFileBodyAsync(sharedFileId);

            if (sharedFileBody is null)
            {
                return null;
            }

            return sharedFileBody.File;
        }

        public async Task<List<SharedFileView>> GetLatestAsync(Guid userId, int number)
        {
            var response = await _repository.SharedFile.GetLatestAsync(userId, number, false);

            return response;
        }
    }
}
