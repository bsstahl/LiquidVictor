using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class SlideExtensions
    {
        public static IEnumerable<KeyValuePair<int, ContentItem>> TextContentItems(this IEnumerable<KeyValuePair<int, ContentItem>> contentItems)
        {
            return contentItems.Where(c => c.Value.IsText());
        }

        public static IEnumerable<KeyValuePair<int, ContentItem>> ImageContentItems(this IEnumerable<KeyValuePair<int, ContentItem>> contentItems)
        {
            return contentItems.Where(c => c.Value.IsImage());
        }

        public static bool IsImage(this ContentItem contentItem)
        {
            return contentItem.ContentType.ToLower().StartsWith("image");
        }

        public static bool IsText(this ContentItem contentItem)
        {
            return contentItem.ContentType.ToLower().StartsWith("text");
        }
    }
}
