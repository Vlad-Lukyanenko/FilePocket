using System.ComponentModel;
using System.Net;
using FilePocket.Application.IntegrationTests.Common;
using FilePocket.Application.IntegrationTests.Common.Extensions;
using FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication;
using FilePocket.Application.IntegrationTests.Common.Utils;
using FluentAssertions;
using Xunit;

namespace FilePocket.Application.IntegrationTests.Api.Files.Upload;

[DisplayName("UploadFileEndpointTests:Validation")]
public class UploadFileEndpointValidationTests(FilePocketWebAppFactory factory) : UploadFileEndpointTests(factory)
{
    [Fact]
    public async Task UploadFile_ShouldReturnBadRequest_When_UserUploadsAnEmptyFile()
    {
        var signUpUser = await ClientAuthenticationFixture.SignUpUserUsingJwtSecurity(FilePocketWebAppFactory);
        var signUpApiClient = signUpUser.JwtAuthenticatedClient;

        var fileUnderTest = FileUnderTestBuilder.CreateOne(
            fileSizeInBytes: 0, fileName: "file-empty-api-test", fileExtension: "txt", userId: signUpUser.JwtTokenUserId);

        var response = await signUpApiClient.PostAsync(ApiFilesEndpointUri, fileUnderTest.MultipartFormDataContent);
        response.EnsureBadRequest();
    }

    [Fact]
    public async Task UploadFile_ShouldReturnBadRequest_When_UserUploadsFileWhichExceedsUsedStorageCapacity()
    {
        var signUpUser = await ClientAuthenticationFixture.SignUpUserUsingJwtSecurity(FilePocketWebAppFactory);
        var signUpApiClient = signUpUser.JwtAuthenticatedClient;

        var fileSize = AccountConsumptionSettings.Storage.CapacityMb.GetFileSizeInBytes() + 1;
        var fileUnderTest = FileUnderTestBuilder.CreateOne(
            fileSizeInBytes: (long) fileSize, fileName: "file-exceeding-limit-api-test",  fileExtension: "txt", signUpUser.JwtTokenUserId);

        var response = await signUpApiClient.PostAsync(ApiFilesEndpointUri, fileUnderTest.MultipartFormDataContent);
        response.EnsureBadRequest();
    }
}