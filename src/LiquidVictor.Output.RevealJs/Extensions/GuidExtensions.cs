using System;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class GuidExtensions
    {
        public static string AsIdAnchor(this Guid id)
        {
            return id.AsIdAnchor(String.Empty);
        }

        public static string AsIdAnchor(this Guid id, string innerContent)
        {
            var content = string.IsNullOrEmpty(innerContent)
                ? "&nbsp;"
                : innerContent;
            return $"<a id=\"{id}\">{content}</a>";
        }

    }
}
