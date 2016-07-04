﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ClassLibrary1.Configuration;
using ClassLibrary1.Models;

namespace ClassLibrary1
{
    public class RequireRendererConfiguration
    {
        private string baseUrl = string.Empty;

        private string requireJsUrl = string.Empty;

        private string urlArgs = null;

        private string entryPointRoot = "~/Scripts/";

        private bool loadOverrides = true;

        private int waitSeconds = 7;

        private IList<string> configPaths = new[] { "~/RequireJS.json" };

        public RequireRendererConfiguration()
        {
            LocaleSelector = helper => "en"; // default the locale to "en"
            ProcessConfig = config => { };
            ProcessOptions = options => { };
        }

        /// <summary>
        /// Scripts base url
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return baseUrl;
            }

            set
            {
                baseUrl = value;
            }
        }

        /// <summary>
        /// RequireJS javascript file url
        /// </summary>
        public string RequireJsUrl
        {
            get
            {
                return this.requireJsUrl;
            }

            set
            {
                this.requireJsUrl = value;
            }
        }

        /// <summary>
        /// Value passed to the urlArgs setting of RequireJS
        /// </summary>
        public string UrlArgs
        {
            get
            {
                return urlArgs;
            }

            set
            {
                urlArgs = value;
            }
        }

        /// <summary>
        /// Value passed to the urlArgs setting of RequireJS
        /// </summary>
        public int WaitSeconds
        {
            get
            {
                return waitSeconds;
            }
            set { waitSeconds = value; }
        }

        /// <summary>
        /// List of RequireJS config file paths
        /// </summary>
        public IList<string> ConfigurationFiles
        {
            get
            {
                return configPaths;
            }

            set
            {
                configPaths = value;
            }
        }

        /// <summary>
        /// Scripts folder relative path (ex. ~/Scripts/)
        /// </summary>
        public string EntryPointRoot
        {
            get
            {
                return entryPointRoot;
            }

            set
            {
                entryPointRoot = value;
            }
        }

        /// <summary>
        /// Logger for library output
        /// </summary>
        public IRequireJsLogger Logger { get; set; }

        /// <summary>
        /// A value indicating whether overrides generated by the AutoCompressor should be loaded.
        /// </summary>
        public bool LoadOverrides
        {
            get
            {
                return loadOverrides;
            }

            set
            {
                loadOverrides = value;
            }
        }

        public ConfigCachingPolicy ConfigCachingPolicy { get; set; }

        public bool CacheConfigObject { get; set; }

        /// <summary>
        /// Gets or sets a function that returns the current locale in short format (ex. "en")
        /// </summary>
        public Func<IHtmlHelper, string> LocaleSelector { get; set; }

        public Action<JsonRequireOutput> ProcessConfig { get; set; }

        public Action<JsonRequireOptions> ProcessOptions { get; set; }
    }
}
