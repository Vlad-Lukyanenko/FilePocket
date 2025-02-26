﻿using AutoMapper;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.WebApi.Mapping;

public class Profiles : Profile
{
    public Profiles()
    {
        CreateMap<UserRegistrationModel, User>();

        CreateMap<FileMetadata, FileUploadInfo>();
        CreateMap<FileMetadata, FileResponseModel>();

        CreateMap<PocketModel, Pocket>().ReverseMap();
        CreateMap<PocketForManipulationsModel, Pocket>();

        CreateMap<FolderModel?, Folder?>().ReverseMap();

        CreateMap<SharedFileModel?, SharedFile?>().ReverseMap();

        CreateMap<Bookmark, BookmarkModel>().ReverseMap();
    }
}
