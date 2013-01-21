using AikidoWebsite.Data.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AikidoWebsite.Web.Extensions {
    
    public static class HtmlExtensions {

        //public static MvcHtmlString DropDownList<T>(this HtmlHelper htmlHelper, string name, object htmlAttributes) where T : Enum {
        //    var seletItemValues = Enum.GetValues(typeof(T))
        //                        .Cast<T>()
        //                        .Select(e => new SelectListItem { Text = e.ToString(), Value = (Convert.ToInt32(e)).ToString() });

        //    return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name, seletItemValues, htmlAttributes);
        //}

        public static MvcHtmlString PublikumDropDown(this HtmlHelper htmlHelper, string name, string dataBind = null) {
            var seletItemValues = Enum.GetValues(typeof(Publikum))
                                .Cast<Publikum>()
                                .Select(e => new SelectListItem { Text = e.ToString(), Value = (Convert.ToInt32(e)).ToString(), Selected = e.Equals(Publikum.Alle) });

            object htmlAttributes = null;
            if (dataBind != null) {
                htmlAttributes = new { @data_bind = dataBind };
            }

            return System.Web.Mvc.Html.SelectExtensions.DropDownList(htmlHelper, name, seletItemValues, htmlAttributes);
        }

    }
}