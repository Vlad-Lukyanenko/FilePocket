﻿namespace FilePocket.Admin.Models.Authentication;

public class RegistrationModel
{
    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }

    public ICollection<string>? Roles { get; set; }
}
