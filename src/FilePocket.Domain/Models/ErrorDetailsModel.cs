﻿using System.Text.Json;

namespace FilePocket.Domain.Models;

public class ErrorDetailsModel
{
    public int StatusCode { get; set; }

    public string? Message { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}
