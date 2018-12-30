using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class ImageLeft : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public ImageLeft(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(Slide slide)
        {
            var result = new StringBuilder();
            result.AppendLine("<section>");
            result.AppendLine($"<h1>{slide.Title}</h1>");
            result.Append("<table><tr>");

            var images = slide.ContentItems.ImageContentItems().OrderBy(c => c.Key);
            result.AppendLine("<td width=\"60%\">");
            foreach (var image in images)
                result.AppendLine($"<img alt=\"{image.Value.FileName}\" src=\"data:{image.Value.ContentType};base64,{image.Value.Content.AsBase64String()}\" />");
            result.AppendLine("</td>");

            var textContentItems = slide.ContentItems.TextContentItems().OrderBy(c => c.Key);
            result.AppendLine("<td style=\"vertical-align:top;\">");
            foreach (var textContentItem in textContentItems)
                result.AppendLine(Markdig.Markdown.ToHtml(textContentItem.Value.Content.AsString(), _pipeline));
            result.AppendLine("</td>");

            result.Append("</tr></table>");
            result.AppendLine("</section>");

            return result.ToString();
        }
    }
}
