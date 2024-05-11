using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Entities;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.ImageLeft
{
    public class Engine : ILayoutStrategy
    {
        readonly Markdig.MarkdownPipeline _pipeline;
        readonly Transition _presentationDefaultTransition;
        // readonly BuilderOptions _builderOptions;

        public Engine(Markdig.MarkdownPipeline pipeline, Transition presentationDefaultTransition, BuilderOptions _)
        {
            _pipeline = pipeline;
            _presentationDefaultTransition = presentationDefaultTransition;
            // _builderOptions = builderOptions;
        }

        public string Layout(Slide slide, int zeroBasedIndex)
        {
            var sb = new StringBuilder();

            sb.AppendLine(slide.AsStartSlideSection(_presentationDefaultTransition));

            sb.AppendLine(slide.Title.AsTitleBlock(slide.Id));
            sb.AppendLine(slide.Layout.AsComment());
            sb.AppendLine(slide.ContentItems.AsComments());
            sb.AppendLine(slide.Notes.AsNotesSection(_pipeline));

            sb.Append("<table><tr>");

            var images = slide.ContentItems.ImageContentItems().OrderBy(c => c.Key);
            sb.AppendLine("<td align=\"right\">");
            foreach (var image in images)
            {
                sb.Append("<img");
                sb.Append($" src=\"{image.Value.RelativePathToImage()}\"");
                sb.Append($" alt=\"{image.Value.FileName}\"");
                sb.AppendLine(" />");
            }
            sb.AppendLine("</td>");

            var textContentItems = slide.ContentItems.TextContentItems().OrderBy(c => c.Key);
            sb.AppendLine("<td style=\"vertical-align:top; text-align: left;\">");
            foreach (var textContentItem in textContentItems)
                sb.AppendLine(Markdig.Markdown.ToHtml(textContentItem.Value.Content.AsString(), _pipeline));
            sb.AppendLine("</td>");

            sb.Append("</tr></table>");
            sb.AppendLine("</section>");

            return sb.ToString();
        }
    }
}
