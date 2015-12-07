using RequireJsNet.Helpers;
using System;
using System.IO;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.PlatformAbstractions;

namespace RequireJsNet.EntryPointResolver
{
    public class DefaultEntryPointResolver : IEntryPointResolver
    {
        private const string DefaultArea = "Common";
        private IHostingEnvironment _hostEnv;
        private IApplicationEnvironment _appEnv;
        private IUrlHelper _urlHelper;


        public virtual string Resolve(ViewContext viewContext, string baseUrl, string entryPointRoot)
        {
            var routingInfo = viewContext.GetRoutingInfo();
            var rootUrl = string.Empty;
            var withBaseUrl = true;
            //var server = viewContext.HttpContext.Server;

            _hostEnv = (IHostingEnvironment)viewContext.HttpContext.RequestServices.GetService(typeof (IHostingEnvironment));
            _appEnv = (IApplicationEnvironment)viewContext.HttpContext.RequestServices.GetService(typeof(IApplicationEnvironment));
            _urlHelper = (IUrlHelper)viewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelper));

            var virtualAppRoot = viewContext.HttpContext.Request.PathBase;

            if (String.IsNullOrWhiteSpace(entryPointRoot))
            {
                entryPointRoot = baseUrl;            
            }


            var resolvedEntryPointRoot = entryPointRoot.StartsWith("~")
                ? entryPointRoot.Replace("~", virtualAppRoot)
                : entryPointRoot;

            var resolvedBaseUrl = baseUrl.StartsWith("~")
                ? baseUrl.Replace("~", virtualAppRoot)
                : baseUrl;


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

            var entryPointTemplates = new[]
            {
                "Controllers/{0}/" + routingInfo.Controller + "/" + routingInfo.Action,
                "Controllers/{0}/" + routingInfo.Controller + "/" + routingInfo.Controller + "-" + routingInfo.Action
            };

            var areas = new[]
            {
                routingInfo.Area,
                DefaultArea
            };

            foreach (var entryPointTmpl in entryPointTemplates)
            {
                foreach (var area in areas)
                {
                    var entryPoint = string.Format(entryPointTmpl, routingInfo.Area).ToModuleName();
                    var filePath = Path.Combine(_hostEnv.WebRootPath, _hostEnv.MapPath(resolvedEntryPointRoot + entryPoint + ".js"));

                    if (File.Exists(filePath))
                    {
                        var computedEntry = GetEntryPoint(filePath, resolvedBaseUrl);
                        return withBaseUrl ? computedEntry : rootUrl + computedEntry;
                    }
                }
            }

            // search for controller/action.js in current area
            //var entryPointTmpl = "Controllers/{0}/" + routingInfo.Controller + "/" + routingInfo.Action;

            //var entryPoint = string.Format(entryPointTmpl, routingInfo.Area).ToModuleName();
            //var filePath = _hostEnv.MapPath(resolvedEntryPointRoot + entryPoint + ".js");

            //if (File.Exists(filePath))
            //{
            //    var computedEntry = GetEntryPoint(filePath, resolvedBaseUrl);
            //    return withBaseUrl ? computedEntry : rootUrl + computedEntry;
            //}

            //// search for controller/action.js in common area
            //entryPoint = string.Format(entryPointTmpl, DefaultArea).ToModuleName();
            //filePath = _hostEnv.MapPath(entryPointRoot + entryPoint + ".js");

            //if (File.Exists(filePath))
            //{
            //    var computedEntry = GetEntryPoint(filePath, resolvedBaseUrl);
            //    return withBaseUrl ? computedEntry : rootUrl + computedEntry;
            //}

            //// search for controller/controller-action.js in current area
            ////entryPointTmpl = "Controllers/{0}/" + routingInfo.Controller + "/" + routingInfo.Controller + "-" + routingInfo.Action;
            //entryPoint = string.Format(entryPointTmpl, routingInfo.Area).ToModuleName();
            //filePath = _hostEnv.MapPath(resolvedEntryPointRoot + entryPoint + ".js");

            //if (File.Exists(filePath))
            //{
            //    var computedEntry = GetEntryPoint(filePath, resolvedBaseUrl);
            //    return withBaseUrl ? computedEntry : rootUrl + computedEntry;
            //}

            //// search for controller/controller-action.js in common area
            //entryPoint = string.Format(entryPointTmpl, DefaultArea).ToModuleName();
            //filePath = _hostEnv.MapPath(resolvedEntryPointRoot + entryPoint + ".js");

            //if (File.Exists(filePath))
            //{
            //    var computedEntry = GetEntryPoint(filePath, resolvedBaseUrl);
            //    return withBaseUrl ? computedEntry : rootUrl + computedEntry;
            //}

            return null;
        }

        private string GetEntryPoint(string filePath, string root)
        {

            var fileName = PathHelpers.GetExactFilePath(filePath);
            var folder = _hostEnv.MapPath(root);
            return PathHelpers.GetRequireRelativePath(folder, fileName);
        }
    }
}
