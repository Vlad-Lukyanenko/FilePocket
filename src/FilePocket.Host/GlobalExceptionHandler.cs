﻿using System.ComponentModel.DataAnnotations;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Net;
using FilePocket.Application.Exceptions;
using FilePocket.Application.Interfaces.Services;

namespace FilePocket.Host;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILoggerService _logger;
    private Serilog.Core.Logger _log;

    public GlobalExceptionHandler(ILoggerService logger)
    {
        _log = new LoggerConfiguration()
                .WriteTo.MongoDBBson("mongodb://212.47.70.105:27017/logs")
                .CreateLogger();

        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";
        var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

        httpContext.Response.StatusCode = contextFeature!.Error switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            BadRequestException => StatusCodes.Status400BadRequest,
            ValidationException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        _log.Error($"Something went wrong: {exception.Message}");

        var errorDetails = new ErrorDetailsModel
        {
            StatusCode = httpContext.Response.StatusCode,
            Message = contextFeature.Error.Message
        };

        await httpContext.Response.WriteAsync(errorDetails.ToString(), cancellationToken);

        return true;
    }
}
