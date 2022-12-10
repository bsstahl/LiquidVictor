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

namespace LiquidVictor.Output.RevealJs.Layout.MultiSlide
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
            var sb = new StringBuilder();

            sb.AppendLine(slide.AsStartSlideSection(_presentationDefaultTransition));
            sb.AppendLine(slide.Layout.AsComment());
            sb.AppendLine($"In the '{slide.Layout}' layout, the slide title is ignored in favor of each of the content item titles".AsComment());
            sb.AppendLine(slide.Notes.AsNotesSection(_pipeline));
            sb.AppendLine(slide.Id.AsIdAnchor());

            foreach (var contentItem in slide.ContentItems.OrderBy(c => c.Key))
            {
                sb.AppendLine(contentItem.Value.AsStartContentItemSlideSection());

                var titleHeadingLevel = (contentItem.Key == 0) ? 1 : 3;
                sb.AppendLine(contentItem.Value.Title.AsTitleHeading(titleHeadingLevel));
                sb.AppendLine(contentItem.Value.Id.ToString().AsComment("ContentItem"));

                // Deep links into vertical slides need to be done using 
                // relative links (i.e. slide indexes such as #/0/3 -- 
                // the 3rd vertical inside the 1st slide in the deck)
                sb.AppendLine($"To deep link to this location, use index.html#/{zeroBasedIndex}/{contentItem.Key}".AsComment());

                if (contentItem.Value.IsText())
                    sb.AppendLine(Markdig.Markdown.ToHtml(contentItem.Value.Content.AsString(), _pipeline));
                else if (contentItem.Value.IsImage())
                    sb.AppendLine($"<img alt=\"{contentItem.Value.FileName}\" src=\"{contentItem.Value.RelativePathToImage()}\" />");
                else
                    throw new NotSupportedException("Only Text and Image content is currently supported");
                
                sb.AppendLine("</section>");
            }

            sb.AppendLine("</section>");

            return sb.ToString();
        }
    }
}
