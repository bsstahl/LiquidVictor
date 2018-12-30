using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.MultiColumn
{
    public class Engine : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public Engine(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(Slide slide)
        {
            var result = new StringBuilder();
            result.AppendLine("<section>");
            result.AppendLine($"<h1>{slide.Title}</h1>");
            result.Append("<table><tr>");

            foreach (var contentItem in slide.ContentItems.OrderBy(c => c.Key))
            {
                result.AppendLine("<td style=\"vertical-align:top;\">");
                if (contentItem.Value.IsText())
                    result.AppendLine(Markdig.Markdown.ToHtml(contentItem.Value.Content.AsString(), _pipeline));
                else if (contentItem.Value.IsImage())
                    result.AppendLine($"<img alt=\"{contentItem.Value.FileName}\" src=\"data:{contentItem.Value.ContentType};base64,{contentItem.Value.Content.AsBase64String()}\" />");
                else
                    throw new NotSupportedException("Only Text and Image content is currently supported");
                result.AppendLine("</td>");
            }

            result.Append("</tr></table>");
            result.AppendLine("</section>");

            return result.ToString();
        }
    }
}
