using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Output.RevealJs.Interfaces;
using Markdig;

namespace LiquidVictor.Output.RevealJs.Layout.FullPage
{
    public class Engine : ILayoutStrategy
    {
        MarkdownPipeline _pipeline;

        public Engine()
            :this(new MarkdownPipelineBuilder().UseAdvancedExtensions().Build())
        { }

        public Engine(Markdig.MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public string Layout(Slide slide)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<section>");

            if (!string.IsNullOrWhiteSpace(slide.Title))
                sb.AppendLine($"<h1>{slide.Title}</h1>");

            if ((slide.ContentItems != null) && (slide.ContentItems.Any()))
            {
                var firstItem = slide.ContentItems
                    .OrderBy(c => c.Key)
                    .First();

                if (firstItem.Value.IsText())
                {
                    var textContent = firstItem.Value.Content.AsString();
                    var content = string.Join("\r\n", textContent);
                    sb.AppendLine(Markdig.Markdown.ToHtml(content, _pipeline));
                }
                else if (firstItem.Value.IsImage())
                {
                    string imageTitle = firstItem.Value.Title ?? "Image";
                    string imageContent = firstItem.Value.Content.AsBase64String();
                    string contentType = firstItem.Value.ContentType;
                    sb.AppendLine($"<img alt=\"{imageTitle}\" src=\"data:{contentType};base64,{imageContent}\" />");
                }
                else
                    throw new Exceptions.ContentTypeNotSupportedException(firstItem.Value.ContentType);
            }

            sb.AppendLine("</section>\r\n");

            return sb.ToString();
        }
    }
}
