using RequireJsNet.Helpers;
using System;
using System.IO;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.PlatformAbstractions;

namespace RequireJsNet.EntryPointResolver
{
    public class DefaultEntryPointResolver : IEntryPointResolver
    {
        private const string DefaultArea = "Common";
        private IHostingEnvironment _hostEnv;


        public virtual string Resolve(ViewContext viewContext, string baseUrl, string entryPointRoot)
        {
            var routingInfo = viewContext.GetRoutingInfo();
            var rootUrl = string.Empty;
            var withBaseUrl = true;

            _hostEnv = (IHostingEnvironment)viewContext.HttpContext.RequestServices.GetService(typeof (IHostingEnvironment));

            var virtualAppRoot = viewContext.HttpContext.Request.PathBase;

            if (string.IsNullOrWhiteSpace(entryPointRoot))
            {
                entryPointRoot = baseUrl;            
            }

            if (!entryPointRoot.EndsWith("/"))
            {
                entryPointRoot = entryPointRoot + "/";
            }

            // align baseUrl and entryPointUrl to allow comparison
            var resolvedBaseUrl = baseUrl.StartsWith("~")
                ? baseUrl.Replace("~", virtualAppRoot)
                : baseUrl;

            var resolvedEntryPointRoot = entryPointRoot.StartsWith("~")
                ? entryPointRoot.Replace("~", virtualAppRoot)
                : entryPointRoot;


            // compare entryPoint and baseUrl
            if (resolvedEntryPointRoot != resolvedBaseUrl)
            {
                // entryPointRoot is different from default.
                if ((entryPointRoot.StartsWith("~") || entryPointRoot.StartsWith("/")))
                {
                    // entryPointRoot is defined as root relative, do not use with baseUrl
                    withBaseUrl = false;
                    rootUrl = resolvedBaseUrl;
                }
                else
                {
                    // entryPointRoot is defined relative to baseUrl; prepend baseUrl
                    resolvedEntryPointRoot = resolvedBaseUrl + entryPointRoot;
                }                
            }

            // define entry point location conventions
            var entryPointTemplates = new[]
            {
                "Controllers/{0}/" + routingInfo.Controller + "/" + routingInfo.Action,
                "Controllers/{0}/" + routingInfo.Controller + "/" + routingInfo.Controller + "-" + routingInfo.Action
            };

            // define areas to search
            var areas = new[]
            {
                routingInfo.Area,
                DefaultArea
            };

            // iterate over the possible locations for the entrypoint script
            foreach (var entryPointTmpl in entryPointTemplates)
            {
                foreach (var area in areas)
                {
                    // determine entrypoint modulename
                    var entryPoint = string.Format(entryPointTmpl, area).ToModuleName();

                    // determine entrypoint disk path
                    var filePath = _hostEnv.WebRootPath + _hostEnv.MapPath(
                        (virtualAppRoot.HasValue
                        ? resolvedEntryPointRoot.Replace(virtualAppRoot.Value, "")
                        : resolvedEntryPointRoot) + entryPoint + ".js");

                    if (File.Exists(filePath))
                    {
                        var computedEntry = GetEntryPoint(filePath, (virtualAppRoot.HasValue
                        ? resolvedBaseUrl.Replace(virtualAppRoot.Value, "")
                        : resolvedBaseUrl));
                        return withBaseUrl ? computedEntry : rootUrl + computedEntry + ".js";
                    }
                }
            }

            return null;
        }

        private string GetEntryPoint(string filePath, string root)
        {

            var fileName = PathHelpers.GetExactFilePath(filePath);
            var folder = _hostEnv.MapPath(root);
            return PathHelpers.GetRequireRelativePath(_hostEnv.WebRootPath + folder, fileName);
        }
    }
}
