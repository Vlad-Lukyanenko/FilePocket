using FastEndpoints;

namespace FilePocket.WebApi.Endpoints.Base
{
    public class BaseEndpointWithoutResponse<TRequest> : Endpoint<TRequest> where TRequest : notnull
    {
        protected Guid UserId => Guid.Parse(HttpContext.User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
    }
}
