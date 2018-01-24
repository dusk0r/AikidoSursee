using AikidoWebsite.Common;
using AikidoWebsite.Common.VCalendar;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AikidoWebsite.Web.Extensions {
    
    public class ICalResult : ActionResult {
        public Calendar Calendar { get; private set; }

        public ICalResult(Calendar calendar) {
            Check.NotNull(calendar, "Kein Kalender übergeben");

            this.Calendar = calendar;
        }

        public override void ExecuteResult(ActionContext context) {
            //context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "text/calendar";
            context.HttpContext.Response.Body.Write(Calendar.ToString());
        }
    }
}