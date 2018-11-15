using System;
using System.Collections.Generic;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class MultiColumn : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public MultiColumn(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(SlideDeck deck, Slide slide)
        {
            var result = new StringBuilder();
            result.AppendLine("<section>");
            result.AppendLine($"<h1>{slide.Title}</h1>");
            result.Append("<table><tr>");

            foreach (var contentItem in slide.ContentText)
            {
                result.AppendLine("<td style=\"vertical-align:top;\">");
                result.AppendLine(Markdig.Markdown.ToHtml(contentItem, _pipeline));
                result.AppendLine("</td>");
            }

            result.Append("</tr></table>");
            result.AppendLine("</section>");

            return result.ToString();
        }
    }
}
