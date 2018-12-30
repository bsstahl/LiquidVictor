using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class Title : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public Title(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(Slide slide)
        {
            var textContentItems = slide.ContentItems
                .TextContentItems()
                .OrderBy(ci => ci.Key)
                .ToArray();

            var markdown = new StringBuilder();
            markdown.AppendLine($"# {slide.Title}");
            markdown.AppendLine($"## {textContentItems[0].Value.Content}");
            markdown.AppendLine($"*{textContentItems[1].Value.Content}*");

            return $"<section>{Markdig.Markdown.ToHtml(markdown.ToString(), _pipeline)}</section>\r\n";
        }
    }
}
