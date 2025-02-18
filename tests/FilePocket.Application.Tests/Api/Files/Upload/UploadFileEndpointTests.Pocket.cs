using FilePocket.Application.IntegrationTests.Common;
using FilePocket.Application.IntegrationTests.Common.Extensions;
using FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication;
using FilePocket.Application.IntegrationTests.Common.Utils;
using FluentAssertions;
using Xunit;

namespace FilePocket.Application.IntegrationTests.Api.Files.Upload;

public class UploadFileEndpointPocketTests(FilePocketWebAppFactory factory) : UploadFileEndpointTests(factory)
{
    [Fact]
    public async Task UploadFile_ShouldReturnFileResponse_WhenUserUploadsFileAttachedToExistingPocket()
    {
        var signUpUser = await ClientAuthenticationFixture.SignUpUserUsingJwtSecurity(FilePocketWebAppFactory);

        var createdPocket = await CreatePocketAsync(
            signUpUser.JwtAuthenticatedClient, signUpUser.JwtTokenUserId);

        var createdFolder = await CreateFolderAsync(
            signUpUser.JwtAuthenticatedClient, signUpUser.JwtTokenUserId, createdPocket.Id);

        var fileUnderTest = FileUnderTestBuilder.CreateOne(
            fileSizeInBytes: FileUnderTestSizes.OneMegabyteAsBytes,
            fileName: "file-with-pocket-api-test",
            fileExtension: "txt",
            userId: signUpUser.JwtTokenUserId,
            pocketId: createdPocket.Id,
            folderId: createdFolder.Id);

        var createdFile = await UploadFileAsync(signUpUser.JwtAuthenticatedClient, fileUnderTest);
        var createdPocketDetails = await GetPocketDetailsAsync(signUpUser.JwtAuthenticatedClient, createdPocket.Id);

        createdFile.PocketId.Should().Be(fileUnderTest.PocketId);
        createdFile.FolderId.Should().Be(fileUnderTest.FolderId);

        createdFile.ShouldBeEquivalentTo(fileUnderTest);
        createdPocketDetails.ShouldBeEquivalentTo(fileUnderTest);
    }

    [Fact]
    public async Task UploadFile_ShouldReturnNotFound_WhenUserUploadsFileAttachedToNonExistingPocket()
    {
        var signUpUser = await ClientAuthenticationFixture.SignUpUserUsingJwtSecurity(FilePocketWebAppFactory);

        var nonExistingPocketId = Guid.NewGuid();
        var fileUnderTest = FileUnderTestBuilder.CreateOne(
            fileSizeInBytes: FileUnderTestSizes.OneMegabyteAsBytes,
            fileName: "file-with-non-existing-pocket-api-test",
            fileExtension: "txt",
            userId: signUpUser.JwtTokenUserId,
            pocketId: nonExistingPocketId);

        var response = await signUpUser.JwtAuthenticatedClient.PostAsync(ApiFilesEndpointUri, fileUnderTest.MultipartFormDataContent);
        response.EnsureNotFound();
    }
}