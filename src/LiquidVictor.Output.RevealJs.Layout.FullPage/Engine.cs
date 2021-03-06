﻿using System;
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
            var sb = new StringBuilder();

            Slide slideToRender = slide.Clone();

            if (slideToRender.MakeSoloImageFullScreen(_config))
            {
                slideToRender.BackgroundContent = slideToRender.ContentItems.Single().Value;
                slideToRender.ContentItems.Clear();
            }

            sb.AppendLine(slideToRender.AsStartSlideSection(_presentationDefaultTransition));

            sb.AppendLine(slideToRender.Title.AsTitle());
            sb.AppendLine(slideToRender.Id.ToString().AsComment());
            sb.AppendLine(slideToRender.Notes.AsNotesSection(_pipeline));

            var contentItems = slideToRender.ContentItems
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
                    sb.AppendLine($"<img alt=\"{contentItem.FileName}\" src=\"{contentItem.RelativePathToImage()}\" />");
                }
                else
                    throw new Exceptions.SlideLayoutException(Enumerations.Layout.FullPage, "Only Image and Text items are supported in this layout");
            }

            sb.AppendLine("</section>\r\n");

            return sb.ToString();
        }
    }
}
