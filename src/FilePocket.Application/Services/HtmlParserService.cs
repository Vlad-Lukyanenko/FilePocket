using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using HtmlAgilityPack;

namespace FilePocket.Application.Services
{
    public class HtmlParserService : IHtmlParserService
    {
        public WebsitePreviewModel GetSiteMetaData(string siteUri)
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(siteUri);

            var metaTags = htmlDoc.DocumentNode.SelectNodes("//meta | //link | //title | //description");

            if (metaTags is null)
            {
                return new WebsitePreviewModel() { };
            }

            var siteMetaData = InitializeWebsitePreview(metaTags);

            return siteMetaData;
        }


        private WebsitePreviewModel InitializeWebsitePreview(HtmlNodeCollection metaTags)
        {
            var websitePreviewModel = new WebsitePreviewModel();

            foreach (var tag in metaTags)
            {
                var tagName = tag.Attributes["name"];
                var tagContent = tag.Attributes["content"];
                var tagProperty = tag.Attributes["property"];
                var tagRel = tag.Attributes["rel"];
                var tagHref = tag.Attributes["href"];

                if (tag.Name.Equals("title") || tag.Name.Equals("description"))
                {
                    websitePreviewModel.Title = tag.InnerText ?? websitePreviewModel.Title;
                    websitePreviewModel.Description = tag.InnerText ?? websitePreviewModel.Description;
                }

                if (tagRel != null && tagHref != null && tagRel.Value.ToLower().Equals("canonical"))
                {
                    websitePreviewModel.PageUrl = tagHref.Value;
                }

                if (tagProperty != null && tagContent != null)
                {
                    switch (tagProperty.Value.ToLower())
                    {
                        case "og:title":
                            websitePreviewModel.Title = string.IsNullOrEmpty(websitePreviewModel.Title) ? tagContent.Value : websitePreviewModel.Title;
                            break;
                        case "og:description":
                            websitePreviewModel.Description = string.IsNullOrEmpty(websitePreviewModel.Description) ? tagContent.Value : websitePreviewModel.Description;
                            break;
                        case "og:image":
                            websitePreviewModel.ImageUrl = tagContent.Value;
                            break;
                        case "og:url":
                            websitePreviewModel.PageUrl = string.IsNullOrEmpty(websitePreviewModel.PageUrl) ? tagContent.Value : websitePreviewModel.PageUrl;
                            break;
                    }
                }
                else if (tagName != null && tagContent != null)
                {
                    switch (tagName.Value.ToLower())
                    {
                        case "title":
                            websitePreviewModel.Title = string.IsNullOrEmpty(websitePreviewModel.Title) ? tagContent.Value : websitePreviewModel.Title;
                            break;
                        case "description":
                            websitePreviewModel.Description = string.IsNullOrEmpty(websitePreviewModel.Description) ? tagContent.Value : websitePreviewModel.Description;
                            break;
                        case "twitter:title":
                            websitePreviewModel.Title = string.IsNullOrEmpty(websitePreviewModel.Title) ? tagContent.Value : websitePreviewModel.Title;
                            break;
                        case "twitter:description":
                            websitePreviewModel.Description = string.IsNullOrEmpty(websitePreviewModel.Description) ? tagContent.Value : websitePreviewModel.Description;
                            break;
                        case "twitter:image":
                            websitePreviewModel.ImageUrl = string.IsNullOrEmpty(websitePreviewModel.ImageUrl) ? tagContent.Value : websitePreviewModel.ImageUrl;
                            break;
                    }
                }
            }

            return websitePreviewModel;
        }
    }
}
