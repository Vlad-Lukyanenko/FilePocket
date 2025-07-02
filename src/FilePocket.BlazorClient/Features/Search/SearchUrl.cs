using FilePocket.BlazorClient.Features.Search.Enums;

namespace FilePocket.BlazorClient.Features.Search;

public static class SearchUrl
{
    public static string GeItemsByPartialName(RequestedItemType itemType, string partialName)
    {
        return $"api/search/{itemType.ToString().ToLower()}/{partialName}";
    }
}
