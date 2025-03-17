using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using MapsterMapper;

namespace FilePocket.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public ProfileService(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProfileModel> GetByIdAsync(Guid id)
    {
        var profile = await _repository.Profile.GetByIdAsync(id);

        return _mapper.Map<ProfileModel>(profile);
    }

    public async Task<ProfileModel> GetByUserIdAsync(Guid userId)
    {
        var profile = await _repository.Profile.GetByUserIdAsync(userId);

        return _mapper.Map<ProfileModel>(profile);
    }

    public async Task<ProfileModel> CreateProfileAsync(ProfileModel profile)
    {
        var profileEntity = _mapper.Map<Profile>(profile);

        _repository.Profile.CreateProfile(profileEntity);
        await _repository.SaveChangesAsync();

        return _mapper.Map<ProfileModel>(profileEntity);
    }
}
