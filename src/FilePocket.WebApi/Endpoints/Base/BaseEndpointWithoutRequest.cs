using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FilePocket.WebApi.Endpoints.Base
{
    public abstract class BaseEndpointWithoutRequest<TResponse> : EndpointWithoutRequest<TResponse>
    {
        protected Guid UserId => Guid.Parse(HttpContext.User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());

        protected Guid? PocketId
        {
            get
            {
                var pocketId = HttpContext.GetRouteValue("pocketId")?.ToString();
                
                return string.IsNullOrWhiteSpace(pocketId) 
                    ? null 
                    : Guid.Parse(pocketId);
            }
        }

        protected Guid? FolderId
        {
            get
            {
                var pocketId = HttpContext.GetRouteValue("folderId")?.ToString();
                
                return string.IsNullOrWhiteSpace(pocketId) 
                    ? null 
                    : Guid.Parse(pocketId);
            }
        }

        protected Guid? ParentFolderId
        {
            get
            {
                var pocketId = HttpContext.GetRouteValue("parentFolderId")?.ToString();
                
                return string.IsNullOrWhiteSpace(pocketId) 
                    ? null 
                    : Guid.Parse(pocketId);
            }
        }
    }
}
