using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;

namespace FilePocket.Infrastructure.Persistence.Repositories;

public class ProfileRepository : RepositoryBase<Profile>, IProfileRepository
{
    public ProfileRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public void CreateProfile(Profile profile)
    {
        Create(profile);
    }
}
