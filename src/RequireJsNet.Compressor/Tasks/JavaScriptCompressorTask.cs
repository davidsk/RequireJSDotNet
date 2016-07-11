﻿// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

using System;
using System.Globalization;

using EcmaScript.NET;

using Yahoo.Yui.Compressor;

namespace RequireJsNet.Compressor
{
    public class JavaScriptCompressorTask : CompressorTask
    {
        private readonly IJavaScriptCompressor compressor;

        private CultureInfo threadCulture;

        public JavaScriptCompressorTask()
            : this(new JavaScriptCompressor())
        {
        }

        public JavaScriptCompressorTask(IJavaScriptCompressor compressor)
            : base(compressor)
        {
            this.compressor = compressor;
            ObfuscateJavaScript = true;
            TaskEngine.ParseAdditionalTaskParameters = this.ParseAdditionalTaskParameters;
            TaskEngine.LogAdditionalTaskParameters = this.LogAdditionalTaskParameters;
            TaskEngine.SetCompressorParameters = this.SetCompressorParameters;
        }

        public bool ObfuscateJavaScript { get; set; }

        public bool PreserveAllSemicolons { get; set; }

        public bool DisableOptimizations { get; set; }

        public string ThreadCulture { get; set; }

        public bool IsEvalIgnored { get; set; }

        public override bool Execute()
        {
            try
            {
                return base.Execute();
            }
            catch (EcmaScriptException ecmaScriptException)
            {
                TaskEngine.Log.LogEcmaError(ecmaScriptException);
                return false;
            }
        }

        private void ParseAdditionalTaskParameters()
        {
            ParseThreadCulture();
        }

        private void SetCompressorParameters()
        {
            compressor.DisableOptimizations = DisableOptimizations;
            compressor.IgnoreEval = IsEvalIgnored;
            compressor.ObfuscateJavascript = ObfuscateJavaScript;
            compressor.PreserveAllSemicolons = PreserveAllSemicolons;
            compressor.ThreadCulture = threadCulture;
            compressor.Encoding = TaskEngine.Encoding;
            compressor.ErrorReporter = new CustomErrorReporter(TaskEngine.LogType);
        }

        private void LogAdditionalTaskParameters()
        {
            TaskEngine.Log.LogBoolean("Obfuscate Javascript", ObfuscateJavaScript);
            TaskEngine.Log.LogBoolean("Preserve semi colons", PreserveAllSemicolons);
            TaskEngine.Log.LogBoolean("Disable optimizations", DisableOptimizations);
            TaskEngine.Log.LogBoolean("Is Eval Ignored", IsEvalIgnored);
            TaskEngine.Log.LogMessage(
                "Line break position: "
                + (LineBreakPosition <= -1 ? "None" : LineBreakPosition.ToString(CultureInfo.InvariantCulture)));
            TaskEngine.Log.LogMessage("Thread Culture: " + threadCulture.DisplayName);
        }

        private void ParseThreadCulture()
        {
            if (string.IsNullOrEmpty(ThreadCulture))
            {
                threadCulture = CultureInfo.InvariantCulture;
                return;
            }

            try
            {
                switch (ThreadCulture.ToLowerInvariant())
                {
                    case "iv":
                    case "ivl":
                    case "invariantculture":
                    case "invariant culture":
                    case "invariant language":
                    case "invariant language (invariant country)":
                        {
                            threadCulture = CultureInfo.InvariantCulture;
                            break;
                        }

                    default:
                        {
                            threadCulture = CultureInfo.CreateSpecificCulture(ThreadCulture);
                            break;
                        }
                }
            }
            catch
            {
                throw new ArgumentException("Thread Culture: " + ThreadCulture + " is invalid.", "ThreadCulture");
            }
        }
    }
}
