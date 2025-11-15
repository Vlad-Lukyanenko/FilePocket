using FilePocket.Application.Extensions;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using MapsterMapper;

namespace FilePocket.Application.Services
{
    public class SharedFileService : ISharedFileService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IMinioService _minioService;

        public SharedFileService(IRepositoryManager repository, IMapper mapper, IMinioService minioService)
        {
            _repository = repository;
            _mapper = mapper;
            _minioService = minioService;
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
            var fileMetadata = await _repository.SharedFile.GetFileBaseMetadataAsync(sharedFileId);

            if (fileMetadata is null)
            {
                return null;
            }

            var args = fileMetadata.GetObjectArgs();

            return await _minioService.GetObjectAsBytesAsync(args);
        }

        public async Task<List<SharedFileView>> GetLatestAsync(Guid userId, int number)
        {
            var response = await _repository.SharedFile.GetLatestAsync(userId, number, false);

            return response;
        }
    }
}
