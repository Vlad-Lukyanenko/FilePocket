﻿using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models
{
    public class NoteCreateModel
    {
        [Required]
        public Guid UserId { get; set; }

        public Guid PocketId { get; set; }

        public Guid? FolderId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}
