﻿using AutoMapper;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.WebApi.Mapping;

public class StorageProfile : Profile
{
    public StorageProfile()
    {
        CreateMap<StorageModel, Storage>().ReverseMap();
        CreateMap<StorageForManipulationsModel, Storage>();
    }
}
