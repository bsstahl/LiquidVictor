using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;
using LiquidVictor.Exceptions;
using LiquidVictor.Output.RevealJs.Interfaces;
using LiquidVictor.Output.RevealJs.Extensions;

namespace LiquidVictor.Output.RevealJs.Layout.ImageWithCaption
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

            string transitionClass = $"{slide.TransitionIn.GetClass(true)} {slide.TransitionOut.GetClass(false)}".Trim();
            if (string.IsNullOrWhiteSpace(transitionClass))
                result.AppendLine("<section>");
            else
                result.AppendLine($"<section data-transition=\"{transitionClass}\">");

            result.AppendLine($"<h1>{slide.Title}</h1>");

            var image = slide.ContentItems.ImageContentItems()
                .OrderBy(c => c.Key).FirstOrDefault();

            var caption = slide.ContentItems.TextContentItems()
                .OrderBy(c => c.Key).FirstOrDefault();

            if (image.HasValue())
                result.AppendLine($"<img alt=\"{image.Value.FileName}\" src=\"data:{image.Value.ContentType};base64,{image.Value.Content.AsBase64String()}\" />");

            if (caption.HasValue())
                result.AppendLine($"<h2>{Markdig.Markdown.ToHtml(caption.Value.Content.AsString(), _pipeline)}</h2>");

            result.AppendLine("</section>");

            return result.ToString();
        }
    }
}