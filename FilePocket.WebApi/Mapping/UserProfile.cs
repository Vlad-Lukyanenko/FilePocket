using AutoMapper;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.WebApi.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRegistrationModel, User>();
    }
}
