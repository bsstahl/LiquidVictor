using System;
using System.Collections.Generic;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class FullPage : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public FullPage(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(SlideDeck deck, Slide slide)
        {
            var content = Markdig.Markdown.ToHtml(slide.ContentText, _pipeline);
            return $"<section><h1>{slide.Title}</h1>{content}</section>\r\n";
        }
    }
}
