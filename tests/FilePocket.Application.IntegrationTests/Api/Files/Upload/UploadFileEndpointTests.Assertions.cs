using System.Net.Http.Json;
using FilePocket.Application.IntegrationTests.Api.Files.Models;
using FilePocket.Domain.Models;
using FluentAssertions;

namespace FilePocket.Application.IntegrationTests.Api.Files.Upload;

internal static class UploadFileEndpointTestsAssertions
{
    internal static void ShouldBeEquivalentTo(this FileResponseModel actualFileResponse, FileInformationUnderTest fileUnderTest)
    {
        actualFileResponse.Should().NotBeNull();

        // populated by backend
        actualFileResponse.Id.Should().NotBeEmpty();
        actualFileResponse.ActualName.Should().NotBeEmpty();

        // based on file sent by user
        actualFileResponse.Should().BeEquivalentTo(new
        {
            OriginalName = fileUnderTest.GetOriginalFileName(),
            fileUnderTest.UserId,
            FileSize = fileUnderTest.FileSizeInBytes,
            fileUnderTest.PocketId,
            fileUnderTest.FolderId
        }, options => options.ExcludingMissingMembers());
    }

    internal static async Task<T> ReadModelAsync<T>(this HttpResponseMessage response) where T: class
    {
        var messageContent = await response.Content.ReadFromJsonAsync<T>();
        messageContent.Should().NotBeNull();
        return messageContent;
    }
}