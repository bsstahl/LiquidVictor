using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.MultiColumn
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

            foreach (var contentItem in slide.ContentItems.OrderBy(c => c.Key))
            {
                sb.AppendLine("<td style=\"vertical-align:top;\">");
                if (contentItem.Value.IsText())
                    sb.AppendLine(Markdig.Markdown.ToHtml(contentItem.Value.Content.AsString(), _pipeline));
                else if (contentItem.Value.IsImage())
                    sb.AppendLine($"<img alt=\"{contentItem.Value.FileName}\" src=\"{contentItem.Value.RelativePathToImage()}\" />");
                else
                    throw new NotSupportedException("Only Text and Image content is currently supported");
                sb.AppendLine("</td>");
            }

            sb.Append("</tr></table>");
            sb.AppendLine("</section>");

            return sb.ToString();
        }
    }
}
