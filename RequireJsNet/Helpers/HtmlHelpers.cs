// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

#if !NET45
    using Microsoft.AspNetCore.Mvc.Rendering;
#else
	using System.Web;
	using System.Web.Mvc;
#endif

using RequireJsNet.Models;

namespace RequireJsNet.Helpers
{
    internal static class HtmlHelpers
    {
        public static RoutingInfo GetRoutingInfo(this ViewContext viewContext)
        {
			#if !NET45
            var area = viewContext.RouteData.Values["area"] != null ? viewContext.RouteData.Values["area"].ToString() : "Root";
            var controller = viewContext.RouteData.Values["controller"].ToString();
            var action = viewContext.RouteData.Values["action"].ToString();
			#else
            var area = viewContext.RouteData.DataTokens["area"] != null ? viewContext.RouteData.DataTokens["area"].ToString() : "Root";
            var controller = viewContext.Controller.ValueProvider.GetValue("controller").RawValue as string;
            var action = viewContext.Controller.ValueProvider.GetValue("action").RawValue as string;
			#endif

            return new RoutingInfo
            {
                Area = area,
                Controller = controller,
                Action = action
            };
        }
    }
}
