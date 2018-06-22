using System.Text;
using Microsoft.AspNetCore.Http;

namespace AikidoWebsite.Web.Extensions
{
    public static class HttpExtensions
    {
        public static string GetBaseUrl(this HttpContext context)
        {
            var builder = new StringBuilder();

            if (context.Request.Headers.ContainsKey("X-Forwarded-Host"))
            {
                builder.Append(GetForwardedProto(context));
                builder.Append("://");
                builder.Append(context.Request.Headers["X-Forwarded-Host"]);
                builder.Append("/");
            }
            else
            {
                builder.Append(context.Request.IsHttps ? "https://" : "http://");
                builder.Append(context.Request.Host);
                builder.Append("/");
            }

            return builder.ToString();
        }

        private static string GetForwardedProto(HttpContext context)
        {
            switch (context.Request.Headers["X-Forwarded-Proto"])
            {
                case "http":
                    return "http";
                case "https":
                default:
                    return "https";
            }
        }
    }
}
