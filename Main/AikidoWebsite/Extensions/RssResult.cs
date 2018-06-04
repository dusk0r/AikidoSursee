using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AikidoWebsite.Web.Extensions
{

    public class RssResult : ActionResult {
        private XDocument rssXml;
        private XElement channel;

        private static readonly XNamespace namespaceAtom = "atom";

        public RssResult(string title, string link, string description) {
            rssXml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            channel = new XElement("channel",
                new XElement("title", title),
                new XElement("link", link),
                new XElement("description", description),
                new XElement("pubDate", FormatDate(DateTime.Now)),
                new XElement(namespaceAtom + "link", 
                    new XAttribute("href", @"http://dallas.example.com/rss.xml"), 
                    new XAttribute("rel", "self"), 
                    new XAttribute("type", "application/rss+xml"))
            );

            rssXml.Add(new XElement("rss", 
                new XAttribute("version", "2.0"), 
                new XAttribute(XNamespace.Xmlns + "atom", @"http://www.w3.org/2005/Atom"),
                channel)
            );
        }

        public override async Task ExecuteResultAsync(ActionContext context) {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "application/rss+xml";
            await context.HttpContext.Response.WriteAsync(rssXml.ToString());
        }

        //public void SetImage(string url, string title, string link) {
        //    // Altes Element löschen
        //    channel.Descendants().Where(d => d.Name.Equals("image")).Remove();

        //    var image = new XElement("image",
        //        new XElement("url", url),
        //        new XElement("title", title),
        //        new XElement("link", link)
        //    );

        //    // Neues Element hinzufügen
        //    channel.Add(image);
        //}

        public void AddItem(string title, string description, string link, string author, string guid, DateTime date) {

            var item = new XElement("item",
                new XElement("title", title),
                new XElement("description", description),
                new XElement("link", link),
                new XElement("author", author),
                new XElement("guid",
                    new XAttribute("isPermaLink", false),
                    guid),
                new XElement("pubDate", FormatDate(date))
            );

            channel.Add(item);
        }

        private string FormatDate(DateTime date) {
            return date.ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                 + " "
                 + date.ToString("%K", CultureInfo.InvariantCulture).Replace(":","");

        }
    }
}