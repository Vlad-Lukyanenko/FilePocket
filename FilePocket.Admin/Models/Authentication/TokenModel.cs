﻿namespace FilePocket.Admin.Models.Authentication;

public class TokenModel
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
}
