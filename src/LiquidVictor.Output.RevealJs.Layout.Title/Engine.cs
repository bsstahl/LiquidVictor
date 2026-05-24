using System;
using System.Collections.Generic;
using System.Globalization;
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
        readonly Transition _presentationDefaultBackgroundTransition;
        readonly BuilderOptions _builderOptions;

        public Engine(Markdig.MarkdownPipeline pipeline, Transition presentationDefaultTransition, Transition presentationDefaultBackgroundTransition, BuilderOptions builderOptions)
        {
            _pipeline = pipeline;
            _presentationDefaultTransition = presentationDefaultTransition;
            _presentationDefaultBackgroundTransition = presentationDefaultBackgroundTransition;
            _builderOptions = builderOptions;
        }

        public string Layout(Slide slide, int zeroBasedIndex)
        {
            ArgumentNullException.ThrowIfNull(slide);

            var textContentItems = slide.ContentItems
                .TextContentItems()
                .OrderBy(ci => ci.Key)
                .ToArray();

            var markdown = new StringBuilder();
            markdown.AppendLine(slide.Title.AsTitleBlock(slide.Id));
            markdown.AppendLine(slide.Layout.AsComment());
            markdown.AppendLine(slide.ContentItems.AsComments());
            markdown.AppendLine(CultureInfo.CurrentCulture, $"## {textContentItems[0].Value.Content.AsString()}");

            string url = textContentItems[2].Value.Content.AsString();
            if (!string.IsNullOrWhiteSpace(url))
                markdown.AppendLine(CultureInfo.CurrentCulture, $"### {url}");

            markdown.AppendLine(CultureInfo.CurrentCulture, $"*{textContentItems[1].Value.Content.AsString()}*");

            string printLinkText = textContentItems[3].Value.Content.AsString();
            if (!string.IsNullOrWhiteSpace(printLinkText))
                markdown.AppendLine(CultureInfo.CurrentCulture, $"##### [{printLinkText}](index.html?print-pdf#/)");

            return $"{slide.AsStartSlideSection(_presentationDefaultTransition, _presentationDefaultBackgroundTransition)}{Markdig.Markdown.ToHtml(markdown.ToString(), _pipeline)}</section>\r\n";
        }
    }
}
