﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.FullPageFragments
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

            sb.AppendLine($"<h1>{slide.Title}</h1><table border=\"0\" width=\"100%\"");
            sb.AppendLine(slide.Id.ToString().AsComment());
            sb.AppendLine(slide.Notes.AsNotesSection(_pipeline));

            var textContentItems = slide.ContentItems.OrderBy(ci => ci.Key).Where(ci => ci.Value.ContentType.ToLower().StartsWith("text"));
            foreach (var contentItem in textContentItems)
            {
                sb.AppendLine("<tr><td>");
                sb.AppendLine(Markdig.Markdown.ToHtml($"{{.fragment}}\r\n{contentItem.Value.Content.AsString()}", _pipeline));
                sb.AppendLine("</td></tr>");
            }
            sb.AppendLine("</table></section>\r\n");

            return sb.ToString();
        }
    }
}
