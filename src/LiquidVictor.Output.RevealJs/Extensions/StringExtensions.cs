using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class StringExtensions
    {
        public static string AsTitle(this string title)
        {
            string result = String.Empty;
            if (!string.IsNullOrWhiteSpace(title))
                result = $"<h1>{title}</h1>";
            return result;
        }

        public static string AsNotesSection(this string notes, Markdig.MarkdownPipeline pipeline)
        {
            return string.IsNullOrWhiteSpace(notes) ? string.Empty : $"<aside class=\"notes\">{Markdig.Markdown.ToHtml(notes, pipeline)}</aside>";
        }

        public static string AsComment(this string comment)
        {
            return string.IsNullOrWhiteSpace(comment) ? string.Empty : $"<!-- {comment} -->";
        }

    }
}
