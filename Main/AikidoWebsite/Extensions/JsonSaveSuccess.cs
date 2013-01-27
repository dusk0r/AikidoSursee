using AikidoWebsite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AikidoWebsite.Web.Extensions {

    public class JsonSaveSuccess : JsonResult {
        public string Id { get; private set; }
        public string Message { get; private set; }

        public JsonSaveSuccess(string id) : this(id, null) { }

        public JsonSaveSuccess(string id, string message) {
            Id = Check.StringHasValue(id, "Id");
            Message = message ?? "Speicher erfolgreich";
        }

        public override void ExecuteResult(ControllerContext context) {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";

            response.Write(Json.Encode(new { Id = this.Id, Message = this.Message }));
        }
    }
}
