// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

using System.Text;

#if !NET45
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web.Mvc;
#endif

namespace RequireJsNet
{
    internal class JavaScriptBuilder
    {
        private const string Type = "application/javascript";

        private readonly TagBuilder scriptTag = new TagBuilder("script");

        private readonly StringBuilder content = new StringBuilder();

        private bool hasNewLine = false;

        public bool TagHasType { get; set; }

		#if !NET45
        public IHtmlContent Render()
        {
            scriptTag.InnerHtml.Clear();
            scriptTag.InnerHtml.AppendHtml(RenderContent());
            scriptTag.TagRenderMode = TagRenderMode.Normal;
            return scriptTag;
        }
		#else

        public string Render()
        {
            scriptTag.InnerHtml = RenderContent();
            return scriptTag.ToString(TagRenderMode.Normal);
        }

		#endif

        public string RenderContent()
        {
            return content.ToString();
        }

        public string RenderStatement()
        {
            if (TagHasType)
            {
                scriptTag.MergeAttribute("type", Type);
            }

			#if !NET45

            scriptTag.InnerHtml.Clear();

            scriptTag.TagRenderMode = TagRenderMode.Normal;

            return scriptTag.ToString();

			#else
			
            scriptTag.InnerHtml = string.Empty;

            return scriptTag.ToString(TagRenderMode.Normal);			
			
			#endif
        }

        public void AddAttributesToStatement(string key, string value)
        {
            scriptTag.MergeAttribute(key, value);
        }

        public void AddStatement(string statement)
        {
            if (!hasNewLine)
            {
                content.AppendLine();
                hasNewLine = true;
            }

            content.AppendLine(statement);
        }
    }
}
