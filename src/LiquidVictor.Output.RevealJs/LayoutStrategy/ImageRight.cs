using System;
using System.Collections.Generic;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class ImageRight : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public ImageRight(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(SlideDeck deck, Slide slide)
        {
            var result = new StringBuilder();
            result.AppendLine("<section>");
            result.AppendLine($"<h1>{slide.Title}</h1>");
            result.Append("<table><tr>");
            result.AppendLine($"<td style=\"vertical-align:top;\">{Markdig.Markdown.ToHtml(slide.ContentText, _pipeline)}</td>");

            var image = slide.PrimaryImage;
            if (image != null)
                result.AppendLine($"<td width=\"60%\"><img alt=\"{image.Name}\" src=\"data:{image.ImageFormat};base64,{image.Content.ToBase64()}\" /></td>");

            result.Append("</tr></table>");
            result.AppendLine("</section>");

            return result.ToString();
        }
    }
}
