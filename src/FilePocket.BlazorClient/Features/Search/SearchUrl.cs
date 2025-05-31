using FilePocket.BlazorClient.Features.Search.Enums;

namespace FilePocket.BlazorClient.Features.Search;

public static class SearchUrl
{
    public static string GeItemsByPartialName(SearchItemType itemType,  string partialName) 
        => $"api/{itemType.ToString().ToLower()}-search/{partialName}";
}
