using System.Net.Http.Json;
using AutoBogus;
using Bogus;
using FilePocket.Application.IntegrationTests.Api.Files.Models;
using FilePocket.Application.IntegrationTests.Common;
using FilePocket.Application.IntegrationTests.Common.Extensions;
using FilePocket.Domain.Models;
using PocketModel = FilePocket.Domain.Models.PocketModel;

namespace FilePocket.Application.IntegrationTests.Api.Files.Upload;

public abstract class UploadFileEndpointTests(FilePocketWebAppFactory factory) : FilePocketWebAppTest(factory)
{
    protected static async Task<FileResponseModel> UploadFileAsync(HttpClient httpClient, FileInformationUnderTest fileUnderTest)
    {
        var response = await httpClient.PostAsync(ApiFilesEndpointUri, fileUnderTest.MultipartFormDataContent);
        return await response.ReadResponseMessageAsync<FileResponseModel>();
    }

    protected static async Task<PocketModel> CreatePocketAsync(HttpClient httpClient, Guid userId)
    {
        var createPocketModel = CreatePocketModelFaker(userId).Generate();
        var response = await httpClient.PostAsJsonAsync(PocketApiEndpointUri, createPocketModel);
        return await response.ReadResponseMessageAsync<PocketModel>();
    }

    protected static async Task<PocketModel> GetPocketAsync(HttpClient httpClient, Guid pocketId)
    {
        var response = await httpClient.GetAsync($"{PocketApiEndpointUri}/{pocketId}");
        return await response.ReadResponseMessageAsync<PocketModel>();
    }
    
    protected static async Task<PocketDetailsModel> GetPocketDetailsAsync(HttpClient httpClient, Guid pocketId)
    {
        var response = await httpClient.GetAsync($"{PocketApiEndpointUri}/{pocketId}/info");
        return await response.ReadResponseMessageAsync<PocketDetailsModel>();
    }

    protected static async Task<FolderModel> CreateFolderAsync(HttpClient httpClient, Guid userId, Guid? pocketId = null, Guid? parentFolderId = null)
    {
        var createFolderModel = CreateFolderModelFaker(userId, pocketId, parentFolderId).Generate();
        var response = await httpClient.PostAsJsonAsync("api/folders", createFolderModel);
        return await response.ReadResponseMessageAsync<FolderModel>();
    }

    private static Faker<FolderModel> CreateFolderModelFaker(Guid userId, Guid? pocketId, Guid? parentFolderId = null) =>
        new AutoFaker<FolderModel>()
            // folder id generation is used for testing purposes only
            .RuleFor(x => x.UserId, userId)
            .RuleFor(x => x.PocketId, pocketId)
            .RuleFor(x => x.ParentFolderId, parentFolderId)
            .RuleFor(x => x.Name, faker => faker.Lorem.Sentence())
            .Ignore(x => x.CreatedAt)
            .Ignore(x => x.UpdatedAt);

    private static Faker<PocketForManipulationsModel> CreatePocketModelFaker(Guid userId) 
        => new AutoFaker<PocketForManipulationsModel>()
            .RuleFor( x => x.UserId, userId)
            .RuleFor(x => x.Name, faker => faker.Lorem.Sentence())
            .RuleFor(x => x.Description, faker => faker.Random.Words(25));
}