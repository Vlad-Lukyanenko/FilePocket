using AutoMapper;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.WebApi.Mapping
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<UserRegistrationModel, User>();

            CreateMap<FileUploadSummary, FileUploadInfo>();
            CreateMap<FileUploadSummary, FileResponseModel>();

            CreateMap<StorageModel, Storage>().ReverseMap();
            CreateMap<StorageForManipulationsModel, Storage>();

            CreateMap<FolderModel, Folder>().ReverseMap();
        }
    }
}
