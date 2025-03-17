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

    public async Task<ProfileModel> CreateProfileAsync(ProfileModel profile)
    {
        var profileEntity = _mapper.Map<Profile>(profile);

        _repository.Profile.CreateProfile(profileEntity);
        await _repository.SaveChangesAsync();

        return _mapper.Map<ProfileModel>(profileEntity);
    }
}
