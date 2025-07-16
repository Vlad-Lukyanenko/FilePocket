using FilePocket.BlazorClient.Shared.Enums;

namespace FilePocket.BlazorClient.Features.Search.Requests
{
    public interface ISearchRequests
    {
        Task<List<T>> GetItemsByPartialNameAsync<T>(RequestedItemType itemType, string partialNameToSearch);
    }
}
