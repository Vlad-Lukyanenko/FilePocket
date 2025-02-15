using System.Net;
using FilePocket.Application.IntegrationTests.Common;
using FilePocket.Application.IntegrationTests.Common.Extensions;
using FilePocket.Application.IntegrationTests.Common.Fixtures.Authentication;
using FilePocket.Application.IntegrationTests.Common.Utils;
using FilePocket.Domain.Models;
using FluentAssertions;
using Xunit;

namespace FilePocket.Application.IntegrationTests.Api.Files.Upload;

public class UploadFileEndpointSequentialTests(FilePocketWebAppFactory factory) : UploadFileEndpointTests(factory)
{
    [Fact]
    public async Task UploadFile_ShouldReturnFileResponseForAll_If_OverallFilesSizeIsBeyondUsedStorageCapacity_When_UserUploadsFilesSequentially()
    {
        var signUpUser = await ClientAuthenticationFixture.SignUpUserUsingJwtSecurity(FilePocketWebAppFactory);
        var signUpApiClient = signUpUser.JwtAuthenticatedClient;

        var lessThanConfiguredCapacity = AccountConsumptionSettings.Storage.CapacityMb - 1;
        var filesUnderTest = FileUnderTestBuilder.CreateMany(
            totalStorageCapacityInMegabytes: (long)lessThanConfiguredCapacity,
            filesAmount: 2,
            fileName: "files-sequential-upload-without-exceeding-api-test",
            fileExtension: "txt",
            userId: signUpUser.JwtTokenUserId,
            pocketId: signUpUser.DefaultPocketId);

        foreach (var test in filesUnderTest)
        {
            var t = test;
            var response = await signUpApiClient.PostAsync(ApiFilesEndpointUri, t.MultipartFormDataContent);
            response.EnsureSuccessStatusCode();

            var model = await response.ReadResponseMessageAsync<FileResponseModel>();
            model.ShouldBeEquivalentTo(test);
        }
    }

    [Fact]
    public async Task UploadFile_ShouldReturnFileResponse_WhenUserFreesStorageCapacityToUploadNewFile()
    {
        var signUpUser = await ClientAuthenticationFixture.SignUpUserUsingJwtSecurity(FilePocketWebAppFactory);
        var signUpApiClient = signUpUser.JwtAuthenticatedClient;

        // let`s arrange that user almost reaches the storage capacity
        var lessThanConfiguredCapacity = AccountConsumptionSettings.Storage.CapacityMb - 1;
        var filesUnderTest = FileUnderTestBuilder.CreateMany(
            totalStorageCapacityInMegabytes: (long)lessThanConfiguredCapacity,
            filesAmount: 2,
            fileName: "files-sequential-upload-without-exceeding-api-test",
            fileExtension: "txt",
            userId: signUpUser.JwtTokenUserId,
            pocketId: signUpUser.DefaultPocketId);

        // so we keep track of all uploaded files
        var fileIdentifiers = new List<Guid>();
        var uploadFileTasks = filesUnderTest.Select(async test =>
        {
            var response = await signUpApiClient.PostAsync(ApiFilesEndpointUri, test.MultipartFormDataContent);
            response.EnsureSuccessStatusCode();

            var model = await response.ReadResponseMessageAsync<FileResponseModel>();
            model.ShouldBeEquivalentTo(test);

            fileIdentifiers.Add(model.Id);
        });

        // execute all uploads
        await Task.WhenAll(uploadFileTasks);
        
        // now we will try to upload a file which exceeds the storage capacity
        var remainingSpaceInBytes = AccountConsumptionSettings.Storage.CapacityMb.GetFileSizeInBytes() - filesUnderTest.Sum(x => x.FileSizeInBytes);

        var fileWhichExceedsSpaceLimit = FileUnderTestBuilder.CreateOne(
            fileSizeInBytes: remainingSpaceInBytes + 1,
            fileName: "file-sequential-upload-after-freeing-space-api-test",
            fileExtension: "txt",
            userId: signUpUser.JwtTokenUserId,
            pocketId: signUpUser.DefaultPocketId);

        // it should fail
        var fileWhichExceedsSpaceLimitResponse = await signUpApiClient.PostAsync(ApiFilesEndpointUri, fileWhichExceedsSpaceLimit.MultipartFormDataContent);
        fileWhichExceedsSpaceLimitResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // so we remove latest to free the space
        var fileToFreeSpace = fileIdentifiers.Last();
        var deleteFileResponse = await signUpApiClient.DeleteAsync($"{ApiFilesEndpointUri}/{fileToFreeSpace}");
        deleteFileResponse.EnsureSuccessStatusCode();

        // now we should be able to upload the file which previously failed :)
        var fileWhichExceedsSpaceLimitAfterFreeingSpaceResponse = await signUpApiClient.PostAsync(ApiFilesEndpointUri, fileWhichExceedsSpaceLimit.MultipartFormDataContent);
        fileWhichExceedsSpaceLimitAfterFreeingSpaceResponse.EnsureSuccessStatusCode();
    }
}