using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class FullPageFragments : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public FullPageFragments(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(SlideDeck deck, Slide slide)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<section>");
            sb.AppendLine($"<h1>{slide.Title}</h1><table border=\"0\" width=\"100%\"");

            var textContentItems = slide.ContentItems.OrderBy(ci => ci.Key).Where(ci => ci.Value.ContentType.ToLower().StartsWith("text"));
            foreach (var contentItem in textContentItems)
            {
                sb.AppendLine("<tr><td>");
                sb.AppendLine(Markdig.Markdown.ToHtml($"{{.fragment}}\r\n{contentItem.Value.Content.AsString()}", _pipeline));
                sb.AppendLine("</td></tr>");
            }
            sb.AppendLine("</table></section>\r\n");

            return sb.ToString();
        }
    }
}
