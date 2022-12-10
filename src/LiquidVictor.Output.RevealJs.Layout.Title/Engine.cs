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

namespace LiquidVictor.Output.RevealJs.Layout.Title
{
    public class Engine : ILayoutStrategy
    {
        readonly Markdig.MarkdownPipeline _pipeline;
        readonly Transition _presentationDefaultTransition;
        readonly BuilderOptions _builderOptions;

        public Engine(Markdig.MarkdownPipeline pipeline, Transition presentationDefaultTransition, BuilderOptions builderOptions)
        {
            _pipeline = pipeline;
            _presentationDefaultTransition = presentationDefaultTransition;
            _builderOptions = builderOptions;
        }

        public string Layout(Slide slide, int zeroBasedIndex)
        {
            var textContentItems = slide.ContentItems
                .TextContentItems()
                .OrderBy(ci => ci.Key)
                .ToArray();

            var markdown = new StringBuilder();
            markdown.AppendLine(slide.Title.AsTitleBlock(slide.Id));
            markdown.AppendLine(slide.Layout.AsComment());
            markdown.AppendLine(slide.ContentItems.AsComments());
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
