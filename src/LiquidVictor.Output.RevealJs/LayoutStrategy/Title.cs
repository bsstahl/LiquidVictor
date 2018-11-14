using System;
using System.Collections.Generic;
using System.Text;
using LiquidVictor.Entities;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class Title : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public Title(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(SlideDeck deck, Slide slide)
        {
            var markdown = new StringBuilder();
            markdown.AppendLine($"# {deck.Title}");
            markdown.AppendLine($"## {deck.SubTitle}");
            markdown.AppendLine($"*{deck.Presenter}*");

            return $"<section>{Markdig.Markdown.ToHtml(markdown.ToString(), _pipeline)}</section>\r\n";
        }
    }
}
