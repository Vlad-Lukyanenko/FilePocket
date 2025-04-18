﻿using FilePocket.BlazorClient.Features.Files.Models;

namespace FilePocket.BlazorClient.Features.SharedFiles.Models
{
    public class SharedFileView
    {
        public Guid SharedFileId { get; set; }
        public FileTypes? FileType { get; set; } = null;
        public string? OriginalName { get; set; } = string.Empty;
        public double FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
