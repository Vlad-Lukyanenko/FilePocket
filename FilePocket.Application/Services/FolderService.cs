﻿using AutoMapper;
using FilePocket.Contracts.Repositories;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.Application.Services
{
    public class FolderService : IFolderService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public FolderService(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task CreateAsync(FolderModel folder)
        {
           var folderEntity = _mapper.Map<Folder>(folder);

            _repository.Folder.Create(folderEntity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid folderId)
        {
            _repository.Folder.Delete(folderId);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<FolderModel>> GetAllAsync(Guid pocketId, Guid? parentFolderId)
        {
            var result = await _repository.Folder.GetAllAsync(pocketId, parentFolderId);

            return _mapper.Map<List<FolderModel>>(result);
        }
    }
}