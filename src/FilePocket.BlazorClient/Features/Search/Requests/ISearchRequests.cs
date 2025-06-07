using FilePocket.BlazorClient.Features.Search.Enums;
using FilePocket.BlazorClient.Features.Search.Models;

namespace FilePocket.BlazorClient.Features.Search.Requests
{
    public interface ISearchRequests
    {
        Task<List<T>> GetItemsByPartialNameAsync<T>(RequestedItemType itemType, string partialNameToSearch);
    }
}
