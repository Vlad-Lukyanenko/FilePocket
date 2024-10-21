namespace FilePocket.Admin.Requests.HttpRequests;

public interface IHttpRequests
{
    Task<HttpResponseMessage> GetAsyncRequest(string requestUri);

    Task<HttpResponseMessage> PostAsyncRequest<TValue>(string requestUri, TValue value);

    Task<HttpResponseMessage> PostHttpContentAsyncRequest<TValue>(string requestUri, TValue content) where TValue : HttpContent;

    Task<HttpResponseMessage> PutAsyncRequest<TValue>(string requestUri, TValue value);

    Task<HttpResponseMessage> DeleteAsyncRequest(string requestUri);
}
