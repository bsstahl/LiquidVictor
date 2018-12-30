using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.ImageRight
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

            result.AppendLine("<td style=\"vertical-align:top;\">");
            var textContentItems = slide.ContentItems
                .TextContentItems().OrderBy(c => c.Key);
            foreach (var contentItem in textContentItems)
                result.AppendLine(Markdig.Markdown.ToHtml(contentItem.Value.Content.AsString(), _pipeline));
            result.AppendLine("</td>");

            var imageContentItems = slide.ContentItems
                .ImageContentItems().OrderBy(c => c.Key);
            foreach (var image in imageContentItems)
                result.AppendLine($"<td width=\"60%\"><img alt=\"{image.Value.FileName}\" src=\"data:{image.Value.ContentType};base64,{image.Value.Content.AsBase64String()}\" /></td>");

            result.Append("</tr></table>");
            result.AppendLine("</section>");

            return result.ToString();
        }
    }
}
