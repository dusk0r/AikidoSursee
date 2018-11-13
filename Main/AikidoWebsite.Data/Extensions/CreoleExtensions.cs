using System;
using Mup;

namespace AikidoWebsite.Data.Extensions
{
    public static class CreoleExtensions
    {
        private static readonly CreoleParser _parser = new CreoleParser();
        private static readonly HtmlWriterVisitor _writer = new HtmlWriterVisitor();

        public static string CreoleToHtml(this string creole)
        {
            if (String.IsNullOrWhiteSpace(creole))
            {
                return String.Empty;
            }

            var parseTree = _parser.Parse(creole);
            return parseTree.Accept(_writer);
        }
    }
}
