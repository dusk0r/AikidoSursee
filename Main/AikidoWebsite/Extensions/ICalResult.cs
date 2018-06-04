using System.Threading.Tasks;
using AikidoWebsite.Common;
using AikidoWebsite.Common.VCalendar;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AikidoWebsite.Web.Extensions
{

    public class ICalResult : ActionResult {
        public Calendar Calendar { get; private set; }

        public ICalResult(Calendar calendar) {
            Check.NotNull(calendar, "Kein Kalender übergeben");

            this.Calendar = calendar;
        }

        public override async Task ExecuteResultAsync(ActionContext context) {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "text/calendar";
            await context.HttpContext.Response.WriteAsync(Calendar.ToString());
        }
    }
}