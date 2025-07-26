using FilePocket.BlazorClient.Features.HtmlParser.Models;

namespace FilePocket.BlazorClient.Features.HtmlParser.Requests
{
    public interface IHtmlParserService
    {
        Task<WebsitePreviewModel> GetWebSitePreviewAsync(string websiteUri);
    }
}
