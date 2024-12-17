using FilePocket.Domain.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;


namespace FilePocket.WebApi
{
    public class RequireHeaderAttribute(IOptions<ClientAppsRequestHeaderSettings> headerSettings) : ActionFilterAttribute
    {

        private readonly ClientAppsRequestHeaderSettings _headerSettings = headerSettings.Value;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(_headerSettings.HeaderName, out var headerValue) || headerValue != _headerSettings.HeaderValue)
            {
                context.Result = new UnauthorizedResult();
            }

            base.OnActionExecuting(context);
        }
    }
}
