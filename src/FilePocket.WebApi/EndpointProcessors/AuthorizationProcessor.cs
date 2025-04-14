using FastEndpoints;
using FilePocket.Domain.Models.Configuration;
using Microsoft.Extensions.Options;

namespace FilePocket.WebApi.EndpointProcessors;

public class AuthorizationProcessor<TRequest> : IPreProcessor<TRequest>
{
    private readonly ApiKeyConfigurationModel _headerSettings;

    public AuthorizationProcessor(IOptions<ApiKeyConfigurationModel> headerSettings)
    {
        _headerSettings = headerSettings.Value;
    }

    public Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken cancellationToken)
    {
        var hasJwtToken = context.HttpContext.User.Identity!.IsAuthenticated;
        var hasApiKey = context.HttpContext.Request.Headers.TryGetValue(_headerSettings.HeaderName, out var potentialApiKey);
        var apiKey = _headerSettings.HeaderValue;

        if (!hasJwtToken && (!hasApiKey || !apiKey.Equals(potentialApiKey)))
        {
            context.HttpContext.Response.SendUnauthorizedAsync();
        }

        return Task.CompletedTask;
    }
}
