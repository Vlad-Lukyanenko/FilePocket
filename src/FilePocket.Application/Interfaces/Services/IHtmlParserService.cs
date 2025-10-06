using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services
{
    public interface IHtmlParserService
    {
        WebsitePreviewModel GetSiteMetaData(string siteUri);
    }
}
