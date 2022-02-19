using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sitemap.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Sitemap.Web.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _url;
        public SitemapController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            _url = $"{httpRequest.Scheme}://{httpRequest.Host}/";
        }

        [Route("sitemap.xml")]
        public IActionResult SitemapXml()
        {
            return DynamicSiteMap();
        }

        private IActionResult DynamicSiteMap()
        {
            var sitemapNode = new List<SitemapNode>();
            sitemapNode.Add(new SitemapNode
            {
                Url = "sitemap.xml/pages"
            });
            string xml = GetSiteMapDocument(sitemapNode, "sitemap");
            return this.Content(xml, "application/xml", Encoding.UTF8);
        }

        private string GetSiteMapDocument(IEnumerable<SitemapNode> sitemapNodes, string type)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "sitemapindex");
            switch (type)
            {
                case "sitemap":
                    foreach (SitemapNode sitemapNode in sitemapNodes)
                    {
                        XElement urlElement = new XElement(
                            xmlns + "sitemap",
                            new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url))
                            );
                        root.Add(urlElement);
                    }
                    break;
                case "url":
                    root = new XElement(xmlns + "urlset");
                    foreach (SitemapNode sitemapNode in sitemapNodes)
                    {
                        XElement urlElement = new XElement(
                            xmlns + "url",
                            new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                            sitemapNode.LastModified == null ? null : new XElement(
                                xmlns + "lastmod",
                                sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                            sitemapNode.SitemapFrequency == null ? null : new XElement(
                                xmlns + "changefreq",
                                sitemapNode.SitemapFrequency.Value.ToString().ToLowerInvariant()),
                            sitemapNode.Priority == null ? null : new XElement(
                                xmlns + "priority",
                                sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                        root.Add(urlElement);
                    }
                    break;
            }

            XDocument document = new XDocument(root);
            return document.ToString();

        }
    }
}

