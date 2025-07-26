namespace FilePocket.BlazorClient.Features.HtmlParser.Models
{
    public class WebsitePreviewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string PageUrl { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public bool ShowPreview { get; set; } = true;
    }
}
