using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;
using LiquidVictor.Exceptions;
using LiquidVictor.Output.RevealJs.Interfaces;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Output.RevealJs.Layout.ImageWithCaption
{
    public class Engine : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        Transition _presentationDefaultTransition;

        public Engine(Markdig.MarkdownPipeline pipeline, Transition presentationDefaultTransition)
        {
            _pipeline = pipeline;
            _presentationDefaultTransition = presentationDefaultTransition;
        }

        public string Layout(Slide slide)
        {
            var sb = new StringBuilder();

            sb.AppendLine(slide.AsStartSlideSection(_presentationDefaultTransition));

            sb.AppendLine($"<h1>{slide.Title}</h1>");
            sb.AppendLine(slide.Id.ToString().AsComment());
            sb.AppendLine(slide.Notes.AsNotesSection(_pipeline));

            var image = slide.ContentItems.ImageContentItems()
                .OrderBy(c => c.Key).FirstOrDefault();

            var caption = slide.ContentItems.TextContentItems()
                .OrderBy(c => c.Key).FirstOrDefault();

            if (image.HasValue())
                sb.AppendLine($"<img alt=\"{image.Value.FileName}\" src=\"data:{image.Value.ContentType};base64,{image.Value.Content.AsBase64String()}\" />");

            if (caption.HasValue())
                sb.AppendLine($"<h2>{Markdig.Markdown.ToHtml(caption.Value.Content.AsString(), _pipeline)}</h2>");

            sb.AppendLine("</section>");

            return sb.ToString();
        }
    }
}