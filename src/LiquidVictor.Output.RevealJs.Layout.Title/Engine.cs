using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.Title
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
            var textContentItems = slide.ContentItems
                .TextContentItems()
                .OrderBy(ci => ci.Key)
                .ToArray();

            var markdown = new StringBuilder();
            markdown.AppendLine($"# {slide.Title}");
            markdown.AppendLine($"## {textContentItems[0].Value.Content.AsString()}");

            string url = textContentItems[2].Value.Content.AsString();
            if (!string.IsNullOrWhiteSpace(url))
                markdown.AppendLine($"### {url}");

            markdown.AppendLine($"*{textContentItems[1].Value.Content.AsString()}*");

            string printLinkText = textContentItems[3].Value.Content.AsString();
            if (!string.IsNullOrWhiteSpace(printLinkText))
                markdown.AppendLine($"##### [{printLinkText}](index.html?print-pdf#/)");

            return $"{slide.AsStartSlideSection(_presentationDefaultTransition)}{Markdig.Markdown.ToHtml(markdown.ToString(), _pipeline)}</section>\r\n";
        }
    }
}
