using System;
using System.Collections.Generic;
using System.Text;
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
            var parseTree = parser.Parse(creole);
            var visitor = new HtmlWriterVisitor();
            return parseTree.Accept(visitor);
        }
    }
}
