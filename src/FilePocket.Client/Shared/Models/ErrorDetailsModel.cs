using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FilePocket.Client.Shared.Models;

public class ErrorDetailsModel
{
    public int StatusCode { get; init; }
    public string? Message { get; init; }

    public static ErrorDetailsModel FromJson(string json) => JsonSerializer.Deserialize<ErrorDetailsModel>(json)!;
    
    public static async Task<ErrorDetailsModel> UnwrapErrorAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var error = FromJson(content);
        return error;
    }

    public void Throw()
        => throw new RequestHandlingErrorException(this);
}

public class RequestHandlingErrorException(ErrorDetailsModel error) : Exception
{
    public ErrorDetailsModel Error { get; } = error;
}