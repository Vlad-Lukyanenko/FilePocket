﻿namespace FilePocket.Application.Exceptions;

public class FileMetadataNotFoundException(Guid fileMetadataId)
    : NotFoundException($"The FileMetadata with id '{fileMetadataId}' doesn't exist in the database.");