using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Layout.FullPage
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
            var sb = new StringBuilder();

            sb.AppendLine(slide.AsStartSlideSection(_presentationDefaultTransition));

            sb.AppendLine($"<h1>{slide.Title}</h1>");
            sb.AppendLine(slide.Id.ToString().AsComment());
            sb.AppendLine(slide.Notes.AsNotesSection(_pipeline));

            var contentItems = slide.ContentItems
                .OrderBy(c => c.Key)
                .Select(c => c.Value);

            foreach (var contentItem in contentItems)
            {
                if (contentItem.IsText())
                {
                    sb.AppendLine(Markdig.Markdown.ToHtml(contentItem.Content.AsString(), _pipeline));
                }
                else if (contentItem.IsImage())
                {
                    sb.AppendLine($"<img alt=\"{contentItem.FileName}\" src=\"data:{contentItem.ContentType};base64,{contentItem.Content.AsBase64String()}\" />");
                }
                else
                    throw new Exceptions.SlideLayoutException(Enumerations.Layout.FullPage, "Only Image and Text items are supported in this layout");
            }

            sb.AppendLine("</section>\r\n");

            return sb.ToString();
        }
    }
}
