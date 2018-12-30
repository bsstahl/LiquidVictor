using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.FullPage
{
    public class Engine : ILayoutStrategy
    {
        Markdig.MarkdownPipeline _pipeline;
        public Engine(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(Slide slide)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<section>");

            sb.AppendLine($"<h1>{slide.Title}</h1>");

            var textContentItems = slide.ContentItems
                .TextContentItems().OrderBy(c => c.Key)
                .Select(c => c.Value.Content.AsString());

            var content = string.Join("\r\n", textContentItems);
            sb.AppendLine(Markdig.Markdown.ToHtml(content, _pipeline));

            sb.AppendLine("</section>\r\n");

            return sb.ToString();
        }
    }
}
