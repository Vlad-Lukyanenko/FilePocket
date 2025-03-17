using FilePocket.Domain.Entities;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IProfileRepository
{
    void CreateProfile(Profile profile);
}
