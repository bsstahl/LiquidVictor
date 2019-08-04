using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.ImageRight
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

            sb.Append("<table><tr>");

            sb.AppendLine("<td style=\"vertical-align:top;\">");
            var textContentItems = slide.ContentItems
                .TextContentItems().OrderBy(c => c.Key);
            foreach (var contentItem in textContentItems)
                sb.AppendLine(Markdig.Markdown.ToHtml(contentItem.Value.Content.AsString(), _pipeline));
            sb.AppendLine("</td>");

            var imageContentItems = slide.ContentItems
                .ImageContentItems().OrderBy(c => c.Key);
            foreach (var image in imageContentItems)
                sb.AppendLine($"<td width=\"60%\"><img alt=\"{image.Value.FileName}\" src=\"{image.Value.RelativePathToImage()}\" /></td>");

            sb.Append("</tr></table>");
            sb.AppendLine("</section>");

            return sb.ToString();
        }
    }
}
