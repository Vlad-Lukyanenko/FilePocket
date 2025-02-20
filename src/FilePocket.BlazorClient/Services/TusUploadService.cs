using FilePocket.BlazorClient.Features;
using TusDotNetClient;

public class TusUploadService
{
    private readonly TusClient _tusClient;
    private readonly string _tusServerUrl;
    private readonly FilePocketApiClient _filePocketApiClient;

    public TusUploadService(FilePocketApiClient filePocketApiClient)
    {
        _tusClient = new TusClient();
        _filePocketApiClient = filePocketApiClient;
        _tusServerUrl = _filePocketApiClient.BaseAddress + "files/tus";
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, Action<long, long> onProgress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        // 1️⃣ Create an upload session using correct method signature
        var uploadUrl = await _tusClient.CreateAsync(
            url: _tusServerUrl,
            uploadLength: fileStream.Length,
            ("filename", fileName) // Using correct metadata format (Tuple array)
        );
        
        // 2️⃣ Start uploading in chunks
        var uploadOperation = _tusClient.UploadAsync(
            uploadUrl,
            fileStream,
            chunkSize: 5.0, // 5MB chunks (TusDotNetClient v1.2.0 uses MB)
            cancellationToken: cancellationToken
        );

        uploadOperation.Progressed += (uploaded, total) => onProgress(uploaded, total);

        // 3️⃣ Handle the TusOperation<List<TusHttpResponse>> result
        var responses = await uploadOperation; // Await completion

        // 4️⃣ Check the responses (all should have StatusCode 204 - NoContent)
        var success = responses.All(response => response.StatusCode == System.Net.HttpStatusCode.NoContent);

        if (!success)
        {
            Console.WriteLine("Upload completed but some chunks may have failed.");
        }

        Console.WriteLine($"Upload completed: {uploadUrl}");
        return uploadUrl;
    }
}