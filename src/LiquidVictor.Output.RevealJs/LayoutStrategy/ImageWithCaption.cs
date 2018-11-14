using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;
using LiquidVictor.Exceptions;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class ImageWithCaption : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public ImageWithCaption(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(SlideDeck deck, Slide slide)
        {
            if (slide.ContentText.Count() != 1)
                throw new SlideLayoutException(Enumerations.Layout.ImageWithCaption, "content must contain exactly one item");

            var result = new StringBuilder();
            result.AppendLine("<section>");

            result.AppendLine($"<h1>{slide.Title}</h1>");

            var image = slide.PrimaryImage;
            if (image != null)
                result.AppendLine($"<img alt=\"{image.Name}\" src=\"data:{image.ImageFormat};base64,{image.Content.ToBase64()}\" />");

            result.AppendLine($"<h2>{Markdig.Markdown.ToHtml(slide.ContentText.Single(), _pipeline)}</h2>");

            result.AppendLine("</section>");

            return result.ToString();
        }
    }
}
