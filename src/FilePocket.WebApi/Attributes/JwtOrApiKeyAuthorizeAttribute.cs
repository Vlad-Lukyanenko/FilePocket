using FilePocket.Domain.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
namespace FilePocket.WebApi.Attributes
{
    public class JwtOrApiKeyAuthorizeAttribute(IOptions<ApiKeyConfigurationModel> headerSettings) : ActionFilterAttribute
    {
        private readonly ApiKeyConfigurationModel _headerSettings = headerSettings.Value;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var hasJwtToken = context.HttpContext.User.Identity!.IsAuthenticated;
            var hasApiKey = context.HttpContext.Request.Headers.TryGetValue(_headerSettings.HeaderName, out var potentialApiKey);
            var apiKey = _headerSettings.HeaderValue;

            if (!hasJwtToken && (!hasApiKey || !apiKey.Equals(potentialApiKey)))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
