﻿namespace FilePocket.BlazorClient.Features.Search.Models
{
    public class BookmarkSearchResponseModel : SearchResponseModel
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
