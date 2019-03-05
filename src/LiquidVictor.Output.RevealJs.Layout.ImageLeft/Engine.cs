using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.ImageLeft
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

            sb.Append("<table><tr>");

            var images = slide.ContentItems.ImageContentItems().OrderBy(c => c.Key);
            sb.AppendLine("<td width=\"60%\">");
            foreach (var image in images)
                sb.AppendLine($"<img alt=\"{image.Value.FileName}\" src=\"data:{image.Value.ContentType};base64,{image.Value.Content.AsBase64String()}\" />");
            sb.AppendLine("</td>");

            var textContentItems = slide.ContentItems.TextContentItems().OrderBy(c => c.Key);
            sb.AppendLine("<td style=\"vertical-align:top;\">");
            foreach (var textContentItem in textContentItems)
                sb.AppendLine(Markdig.Markdown.ToHtml(textContentItem.Value.Content.AsString(), _pipeline));
            sb.AppendLine("</td>");

            sb.Append("</tr></table>");
            sb.AppendLine("</section>");

            return sb.ToString();
        }
    }
}
