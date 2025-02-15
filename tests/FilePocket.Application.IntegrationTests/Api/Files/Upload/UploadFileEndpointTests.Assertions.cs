using FilePocket.Application.IntegrationTests.Api.Files.Models;
using FilePocket.Application.IntegrationTests.Common.Utils;
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

    internal static void ShouldBeEquivalentTo(this PocketModel actualPocket, params FileInformationUnderTest[] filesUnderTest)
    {
        actualPocket.Should().NotBeNull();
        actualPocket.NumberOfFiles.Should().Be(filesUnderTest.Length);
        actualPocket.TotalSize.Should().Be(filesUnderTest.Sum(x => x.FileSizeInBytes));
    }
    
    internal static void ShouldBeEquivalentTo(this PocketDetailsModel actualPocketDetails, params FileInformationUnderTest[] filesUnderTest)
    {
        actualPocketDetails.Should().NotBeNull();
        actualPocketDetails.NumberOfFiles.Should().Be(filesUnderTest.Length);
        actualPocketDetails.TotalFileSize.GetFileSizeInBytes().Should().Be(filesUnderTest.Sum(x => x.FileSizeInBytes));
    }
}