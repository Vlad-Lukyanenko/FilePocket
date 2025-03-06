using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace FilePocket.WebApi.Endpoints.Base;

public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse> where TRequest : notnull
{
    protected Guid UserId => Guid.Parse(HttpContext.User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
}
