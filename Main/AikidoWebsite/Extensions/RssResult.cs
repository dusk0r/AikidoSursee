using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace AikidoWebsite.Web.Extensions {
    
    public class RssResult : ActionResult {
        private XDocument rssXml;
        private XElement channel;

        public RssResult(string title, string link, string description) {
            rssXml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            channel = new XElement("channel",
                new XElement("title", title),
                new XElement("link", link),
                new XElement("description", description),
                new XElement("pubDate", FormatDate(DateTime.Now))
            );

            rssXml.Add(new XElement("rss", 
                new XAttribute("version", "2.0"), 
                channel)
            );
        }

        public override void ExecuteResult(ControllerContext context) {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "application/rss+xml";
            context.HttpContext.Response.Write(rssXml.ToString());
        }

        public void SetImage(string url, string title, string link) {
            // Altes Element löschen
            channel.Descendants().Where(d => d.Name.Equals("image")).Remove();

            var image = new XElement("image",
                new XElement("url", url),
                new XElement("title", title),
                new XElement("link", link)
            );

            // Neues Element hinzufügen
            channel.Add(image);
        }

        public void AddItem(string title, string description, string link, string author, string guid, DateTime date) {
            var item = new XElement("item",
                new XElement("title", title),
                new XElement("description", description),
                new XElement("link", link),
                new XElement("author", author),
                new XElement("guid", guid),
                new XElement("pubDate", FormatDate(date))
            );

            channel.Add(item);
        }

        private string FormatDate(DateTime date) {
            return date.ToString("ddd, d MMM yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);
        }
    }
}