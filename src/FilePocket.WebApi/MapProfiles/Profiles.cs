using FilePocket.Contracts.Home;
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
        }
    }
}