using FilePocket.Application.IntegrationTests.Common;

namespace FilePocket.Application.IntegrationTests.Api.Files.Upload;

public abstract class UploadFileEndpointTests(FilePocketWebAppFactory factory) : FilePocketWebAppTest(factory)
{
    protected const string ApiFilesEndpointUri = "/api/files";
}