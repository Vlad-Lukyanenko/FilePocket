using System.ComponentModel;
using System.Net;
using FilePocket.Application.IntegrationTests.Common;
using FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication;
using FilePocket.Application.IntegrationTests.Common.Utils;
using FluentAssertions;
using Xunit;

namespace FilePocket.Application.IntegrationTests.Api.Files.Upload;

[DisplayName("UploadFileEndpointTests:Parallel")]
public class UploadFileEndpointParallelTests(FilePocketWebAppFactory factory) : UploadFileEndpointTests(factory)
{
    [Fact]
    public async Task UploadFile_ShouldReturnFileResponseOrBadRequest_DependsOn_UsedStorageCapacity_When_UserUploadsMultipleFilesInParallel()
    {
        var signUpUser = await ClientAuthenticationFixture.SignUpUserUsingJwtSecurity(FilePocketWebAppFactory);
        var signUpApiClient = signUpUser.JwtAuthenticatedClient;

        var equalOrGreaterThanConfiguredCapacity = AccountConsumptionSettings.Storage.CapacityMb;
        var filesUnderTest = FileUnderTestBuilder.CreateMany(
            totalStorageCapacityInMegabytes: (long) equalOrGreaterThanConfiguredCapacity,
            filesAmount: 5,
            fileName: "files-parallel-upload-with-exceeding-api-test",
            fileExtension: "txt",
            userId: signUpUser.JwtTokenUserId,
            pocketId: Guid.NewGuid());

        var uploadTasks = filesUnderTest.Select(test =>
            signUpApiClient.PostAsync(ApiFilesEndpointUri, test.MultipartFormDataContent)).ToArray();

        await Task.WhenAll(uploadTasks);

        var failedUploads = uploadTasks.Where(task => task.Result.StatusCode == HttpStatusCode.BadRequest).ToArray();
        failedUploads.Should().NotBeEmpty();

        var successfulUploads = uploadTasks.Where(task => task.Result.StatusCode == HttpStatusCode.OK).ToArray();
        successfulUploads.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UploadFile_ShouldReturnFileResponse_If_OverallFilesSizeIsBeyondUsedStorageCapacity_When_UserUploadsMultipleFilesInParallel()
    {
        var signUpUser = await ClientAuthenticationFixture.SignUpUserUsingJwtSecurity(FilePocketWebAppFactory);
        var signUpApiClient = signUpUser.JwtAuthenticatedClient;

        var lessThanConfiguredCapacity = AccountConsumptionSettings.Storage.CapacityMb - 1;
        var filesUnderTest = FileUnderTestBuilder.CreateMany(
            totalStorageCapacityInMegabytes: (long)lessThanConfiguredCapacity,
            filesAmount: 5,
            fileName: "files-parallel-upload-without-exceeding-api-test",
            fileExtension: "txt",
            userId: signUpUser.JwtTokenUserId,
            pocketId: Guid.NewGuid());

        var uploadTasks = filesUnderTest.Select(test =>
            signUpApiClient.PostAsync(ApiFilesEndpointUri, test.MultipartFormDataContent)).ToArray();

        await Task.WhenAll(uploadTasks);

        var failedUploads = uploadTasks.Where(task => task.Result.StatusCode == HttpStatusCode.BadRequest).ToArray();
        failedUploads.Should().BeEmpty();

        var successfulUploads = uploadTasks.Where(task => task.Result.StatusCode == HttpStatusCode.OK).ToArray();
        successfulUploads.Should().NotBeEmpty();
    }
}