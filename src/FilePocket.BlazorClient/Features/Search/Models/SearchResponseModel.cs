﻿namespace FilePocket.BlazorClient.Features.Search.Models
{
    public class SearchResponseModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid PocketId { get; set; }

        public Guid? FolderId { get; set; }

        public Guid UserId { get; set; }

        public virtual string ItemName { get; set; } = string.Empty;
    }
}
