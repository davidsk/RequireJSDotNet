using RequireJsNet.Helpers;
using System;
using System.IO;

#if !NET45

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.PlatformAbstractions;

#else

using System.Web;
using System.Web.Mvc;

#endif

namespace RequireJsNet.EntryPointResolver
{
    public class DefaultEntryPointResolver : IEntryPointResolver
    {
        private const string DefaultArea = "Common";

		#if !NET45
        private IHostingEnvironment _hostEnv;
		#endif

        public virtual string Resolve(ViewContext viewContext, string baseUrl, string entryPointRoot)
        {
            var routingInfo = viewContext.GetRoutingInfo();

			#if !NET45
            _hostEnv = (IHostingEnvironment)viewContext.HttpContext.RequestServices.GetService(typeof (IHostingEnvironment));

            var pathBase = viewContext.HttpContext.Request.PathBase.ToString();
            var appUriRoot = string.IsNullOrEmpty(pathBase) ? viewContext.HttpContext.Request.Host.ToString() : pathBase;

            var resolvedBaseUrl = ResolvePathToAppRoot(baseUrl, appUriRoot);
            var resolvedEntryPointRoot = string.IsNullOrEmpty(entryPointRoot) ? resolvedBaseUrl : ResolvePathToAppRoot(entryPointRoot, appUriRoot);

			#else
			
			var rootUrl = string.Empty;
            var withBaseUrl = true;
            var server = viewContext.HttpContext.Server;

            if (String.IsNullOrWhiteSpace(entryPointRoot))
            {
                entryPointRoot = baseUrl;            
            }

			var resolvedBaseUrl = UrlHelper.GenerateContentUrl(baseUrl, viewContext.HttpContext);
            var resolvedEntryPointRoot = UrlHelper.GenerateContentUrl(entryPointRoot, viewContext.HttpContext);


            if (resolvedEntryPointRoot != resolvedBaseUrl)
            {
                // entryPointRoot is different from default.
                if ((entryPointRoot.StartsWith("~") || entryPointRoot.StartsWith("/")))
                {
                    // entryPointRoot is defined as root relative, do not use with baseUrl
                    withBaseUrl = false;
                    rootUrl = resolvedEntryPointRoot;
                }
                else
                {
                    // entryPointRoot is defined relative to baseUrl; prepend baseUrl
                    resolvedEntryPointRoot = resolvedBaseUrl + entryPointRoot;
                }                
            }

			#endif

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

          			#if !NET45
		            // determine entrypoint disk path
                    var filePath = _hostEnv.WebRootFileProvider.GetFileInfo(entryPoint + ".js").PhysicalPath;
					#else
					var filePath = server.MapPath(entryPointRoot + entryPoint + ".js");
					#endif
					


                    if (File.Exists(filePath))
                    {
                        // compute the entrypoint relative to the baseUrl
						#if !NET45
                        var computedEntry = GetEntryPoint(filePath, resolvedBaseUrl);
                        return computedEntry;
						#else
						var computedEntry = GetEntryPoint(server, filePath, baseUrl);
		                return withBaseUrl ? computedEntry : rootUrl + computedEntry;
						#endif
                    }
                }
            }

            return null;
        }

		#if !NET45
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
		#else
		
        private static string GetEntryPoint(HttpServerUtilityBase server, string filePath, string root)
        {

            var fileName = PathHelpers.GetExactFilePath(filePath);
            var folder = server.MapPath(root);
            return PathHelpers.GetRequireRelativePath(folder, fileName);
        }		

		#endif
		
    }
}
