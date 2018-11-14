﻿using System;
using System.Collections.Generic;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class FullPageFragments : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public FullPageFragments(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(SlideDeck deck, Slide slide)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<section>");
            sb.AppendLine($"<h1>{slide.Title}</h1>");

            foreach (var contentItem in slide.ContentText)
                sb.AppendLine(Markdig.Markdown.ToHtml($"{{.fragment}}\r\n{contentItem}", _pipeline));

            sb.AppendLine("</section>\r\n");

            return sb.ToString();
        }
    }
}
