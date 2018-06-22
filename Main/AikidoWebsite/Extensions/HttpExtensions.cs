using System.Text;
using Microsoft.AspNetCore.Http;

namespace AikidoWebsite.Web.Extensions
{
    public static class HttpExtensions
    {
        public static string GetBaseUrl(this HttpContext context)
        {
            var builder = new StringBuilder();
            builder.Append(context.Request.IsHttps ? "https://" : "http://");
            builder.Append(context.Request.Host);
            builder.Append("/");

            return builder.ToString();
        }
    }
}
