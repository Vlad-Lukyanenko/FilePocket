﻿namespace FilePocket.Contracts.Folders.Requests
{
    public class CreateEndpointRequest
    {
        public Guid? Id { get; set; }

        public Guid? PocketId { get; set; }

        public Guid? ParentFolderId { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsSelected { get; set; }
    }
}
