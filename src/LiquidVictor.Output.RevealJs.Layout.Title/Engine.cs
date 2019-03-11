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
        Markdig.MarkdownPipeline _pipeline;
        Transition _presentationDefaultTransition;

        public Engine(Markdig.MarkdownPipeline pipeline, Transition presentationDefaultTransition)
        {
            _pipeline = pipeline;
            _presentationDefaultTransition = presentationDefaultTransition;
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

            if (textContentItems.Length > 2)
                markdown.AppendLine($"### {textContentItems[2].Value.Content.AsString()}");

            markdown.AppendLine($"*{textContentItems[1].Value.Content.AsString()}*");


            return $"{slide.AsStartSlideSection(_presentationDefaultTransition)}{Markdig.Markdown.ToHtml(markdown.ToString(), _pipeline)}</section>\r\n";
        }
    }
}
