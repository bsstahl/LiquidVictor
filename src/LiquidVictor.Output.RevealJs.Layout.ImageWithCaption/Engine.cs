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
        readonly Markdig.MarkdownPipeline _pipeline;
        readonly Transition _presentationDefaultTransition;
        readonly Configuration _config;

        public Engine(Markdig.MarkdownPipeline pipeline, Transition presentationDefaultTransition, Configuration config)
        {
            _pipeline = pipeline;
            _presentationDefaultTransition = presentationDefaultTransition;
            _config = config;
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
                sb.AppendLine($"<img alt=\"{image.Value.FileName}\" src=\"{image.Value.RelativePathToImage()}\" />");

            if (caption.HasValue())
                sb.AppendLine($"<h2>{Markdig.Markdown.ToHtml(caption.Value.Content.AsString(), _pipeline)}</h2>");

            sb.AppendLine("</section>");

            return sb.ToString();
        }
    }
}