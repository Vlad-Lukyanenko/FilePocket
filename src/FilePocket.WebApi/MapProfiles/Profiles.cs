using FilePocket.Contracts.Home;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Mapster;

namespace FilePocket.WebApi.MapProfiles
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<SharedFileView, GetRecentlySharedFilesResponse>()
                .Map(dest => dest.SharedFileId, src => src.SharedFileId)
                .Map(dest => dest.FileType, src => src.FileType)
                .Map(dest => dest.OriginalName, src => src.OriginalName);

            config.NewConfig<FileResponseModel, GetRecentlyUploadedFilesResponse>()
               .Map(dest => dest.Id, src => src.Id)
               .Map(dest => dest.PocketId, src => src.PocketId)
               .Map(dest => dest.FolderId, src => src.FolderId)
               .Map(dest => dest.FileType, src => src.FileType)
               .Map(dest => dest.OriginalName, src => src.OriginalName);

            config.NewConfig<FileResponseModel, NoteModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.PocketId, src => src.PocketId)
                .Map(dest => dest.FolderId, src => src.FolderId)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
                .Map(dest => dest.Title, src => src.OriginalName);

            config.NewConfig<FileMetadata, NoteModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.PocketId, src => src.PocketId)
                .Map(dest => dest.FolderId, src => src.FolderId)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
                .Map(dest => dest.Title, src => src.OriginalName);
        }
    }
}