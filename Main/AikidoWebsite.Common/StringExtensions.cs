using System;
using System.Text.RegularExpressions;

namespace AikidoWebsite.Common
{
    public static class StringExtensions
    {
        private static readonly Regex Newlines = new Regex(@"\t|\n|\r", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string Limit(this string str, int length, string tail = "...")
        {
            if (str.Length < length)
            {
                return str;
            }
            else
            {
                return str.Substring(0, length - tail.Length) + tail;
            }
        }

        public static string NullSave(this string str)
        {
            return str ?? "";
        }

        public static string RemoveNewlines(this string str)
        {
            return Newlines.Replace(str, String.Empty);
        }
    }
}
