namespace AikidoWebsite.Common
{
    public static class StringExtensions
    {

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

        public static string RemoveNewline(this string str)
        {
            return str.Replace("\n", "").Replace("\r", "");
        }
    }
}
