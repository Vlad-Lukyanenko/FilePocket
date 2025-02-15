using System.Net.Http.Json;
using FluentAssertions;

namespace FilePocket.Application.IntegrationTests.Common.Extensions;

internal static class HttpResponseMessageExtensions
{
    internal static void EnsureBadRequest(this HttpResponseMessage response)
        => response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    
    internal static void EnsureNotFound(this HttpResponseMessage response)
        => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

    internal static async Task<T> ReadResponseMessageAsync<T>(this HttpResponseMessage response) where T: class
    {
        response.EnsureSuccessStatusCode();
        var messageContent = await response.Content.ReadFromJsonAsync<T>();
        messageContent.Should().NotBeNull();
        return messageContent;
    }
}