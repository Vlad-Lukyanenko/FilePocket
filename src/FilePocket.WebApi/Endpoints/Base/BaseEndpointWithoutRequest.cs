using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace FilePocket.WebApi.Endpoints.Base
{
    public abstract class BaseEndpointWithoutRequest<TResponse> : EndpointWithoutRequest<TResponse>
    {
        protected Guid UserId => Guid.Parse(HttpContext.User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
    }
}
