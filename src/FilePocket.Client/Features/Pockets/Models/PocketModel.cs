﻿using System.ComponentModel.DataAnnotations;

namespace FilePocket.Client.Services.Pockets.Models;

public class PocketModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } 
    public DateTime DateCreated { get; set; }

    public int? NumberOfFiles { get; set; }

    public double? TotalSize { get; set; }
}

