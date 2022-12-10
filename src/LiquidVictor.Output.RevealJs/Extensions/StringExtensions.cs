using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class StringExtensions
    {
        public static string AsTitleBlock(this string title, Guid id, int headingLevel = 1)
        {
            return id.AsIdAnchor(title.AsTitleHeading(headingLevel));
        }

        public static string AsTitleHeading(this string title, int headingLevel)
        {
            if (headingLevel < 1) headingLevel= 1;
            return string.IsNullOrEmpty(title)
                ? string.Empty
                : $"<h{headingLevel}>{title}</h{headingLevel}>";
        }

        public static string AsNotesSection(this string notes, Markdig.MarkdownPipeline pipeline)
        {
            return string.IsNullOrWhiteSpace(notes) ? string.Empty : $"<aside class=\"notes\">{Markdig.Markdown.ToHtml(notes, pipeline)}</aside>";
        }

        public static string AsComment(this string comment, string prefix = null)
        {
            return string.IsNullOrWhiteSpace(comment) 
                ? string.Empty 
                : string.IsNullOrWhiteSpace(prefix)
                    ? $"<!-- {comment} -->"
                    : $"<!-- {prefix}:{comment} -->";
        }

    }
}
