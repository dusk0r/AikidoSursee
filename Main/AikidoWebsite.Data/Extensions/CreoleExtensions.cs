using System;
using Mup;

namespace AikidoWebsite.Data.Extensions
{
    public static class CreoleExtensions
    {
        public static string CreoleToHtml(this string creole)
        {
            if (String.IsNullOrWhiteSpace(creole))
            {
                return String.Empty;
            }

            var parser = new CreoleParser();
            var writer = new HtmlWriterVisitor();

            var parseTree = parser.Parse(creole);
            return parseTree.Accept(writer);
        }
    }
}
