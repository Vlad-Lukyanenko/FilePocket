﻿namespace FilePocket.Shared.Exceptions;

public class NotFoundException : Exception
{
    protected NotFoundException(string message)
        : base(message)
    { 
    }
}
