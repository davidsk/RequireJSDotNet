using RequireJsNet.Helpers;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            _hostEnv = (IHostingEnvironment)viewContext.HttpContext.RequestServices.GetService(typeof (IHostingEnvironment));

            var pathBase = viewContext.HttpContext.Request.PathBase.ToString();
            var appUriRoot = string.IsNullOrEmpty(pathBase) ? viewContext.HttpContext.Request.Host.ToString() : pathBase;

            var resolvedBaseUrl = ResolvePathToAppRoot(baseUrl, appUriRoot);
            var resolvedEntryPointRoot = string.IsNullOrEmpty(entryPointRoot) ? resolvedBaseUrl : ResolvePathToAppRoot(entryPointRoot, appUriRoot);

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
                    var entryPoint = string.Format(resolvedEntryPointRoot + entryPointTmpl, area).ToModuleName();

                    // determine entrypoint disk path
                    var filePath = _hostEnv.WebRootFileProvider.GetFileInfo(entryPoint + ".js").PhysicalPath;

                    if (File.Exists(filePath))
                    {
                        // compute the entrypoint relative to the baseUrl
                        var computedEntry = GetEntryPoint(filePath, resolvedBaseUrl);
                        return computedEntry;
                    }
                }
            }

            return null;
        }

        public static string ResolvePathToAppRoot(string path, string appUriRoot)
        {
            path = path.Trim();

            // check for protocol
            if (path.Contains(appUriRoot))
            {
                // baseUrl is absolute
                path = path.Substring(path.IndexOf(appUriRoot, StringComparison.Ordinal) + appUriRoot.ToString().Length + 1);
            }
            else
            {
                //baseUrl is relative
                if (path.StartsWith("~") || path.StartsWith("/"))
                {
                    path = path.Substring(path.IndexOf("/", StringComparison.Ordinal) + 1);
                }
            }

            return path;
        }

        private string GetEntryPoint(string filePath, string resolvedBaseUrl)
        {

            var fileName = PathHelpers.GetExactFilePath(filePath);
            return PathHelpers.GetRequireRelativePath(Path.Combine(_hostEnv.WebRootPath, resolvedBaseUrl.Replace('/', Path.DirectorySeparatorChar)), fileName);
        }
    }
}
