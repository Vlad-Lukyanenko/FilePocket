
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.HtmlParser
{
    public class GetWebsitePreview : BaseEndpointWithoutRequest<WebsitePreviewModel>
    {
        private readonly IServiceManager _service;

        public GetWebsitePreview(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Get("api/html-parser/website-preview");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var siteUri = Query<string>("siteUri");
            if (string.IsNullOrEmpty(siteUri))
            {
                AddError("SiteUri", "Site URI cannot be null or empty.");
                await SendErrorsAsync(cancellation: cancellationToken);
                return;
            }

            var siteMetaData = _service.HtmlParserService.GetSiteMetaData(siteUri);

            if (siteMetaData == null || !siteMetaData.ShowPreview)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }
            await SendOkAsync(siteMetaData, cancellationToken);
        }
    }
}
