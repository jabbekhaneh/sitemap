using System;

namespace Sitemap.Web.Models
{
    public class SitemapNode
    {
        public SitemapFrequency? SitemapFrequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Url { get; set; }


    }

    public enum SitemapFrequency
    {
        Never,
        Yearly,
        Monthly,
        Weekly,
        Daily,
        Hourly,
        Always
    }
}
