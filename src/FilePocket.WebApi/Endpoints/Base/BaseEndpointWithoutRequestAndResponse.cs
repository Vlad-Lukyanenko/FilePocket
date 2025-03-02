using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace FilePocket.WebApi.Endpoints.Base
{
    public abstract class BaseEndpointWithoutRequestAndResponse : EndpointWithoutRequest
    {
        protected Guid UserId => Guid.Parse(HttpContext.User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
    }

}
