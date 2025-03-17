using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;

namespace FilePocket.Infrastructure.Persistence.Repositories;

public class ProfileRepository : RepositoryBase<Profile>, IProfileRepository
{
    public ProfileRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public async Task<Profile> GetByIdAsync(Guid id)
    {
        return (await FindByCondition(p => p.Id.Equals(id)))!;
    }

    public async Task<Profile> GetByUserIdAsync(Guid userId)
    {
        return (await FindByCondition(p => p.UserId.Equals(userId)))!;
    }

    public void CreateProfile(Profile profile)
    {
        Create(profile);
    }
}
