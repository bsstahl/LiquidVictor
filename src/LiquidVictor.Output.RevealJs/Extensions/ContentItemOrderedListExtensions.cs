using System;
using System.Collections.Generic;
using System.Linq;
using LiquidVictor.Entities;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class ContentItemOrderedListExtensions
    {
        public static IEnumerable<KeyValuePair<int, ContentItem>> TextContentItems(this IEnumerable<KeyValuePair<int, ContentItem>> contentItems)
        {
            return contentItems.Where(c => c.Value.IsText());
        }

        public static IEnumerable<KeyValuePair<int, ContentItem>> ImageContentItems(this IEnumerable<KeyValuePair<int, ContentItem>> contentItems)
        {
            return contentItems.Where(c => c.Value.IsImage());
        }

        public static string AsComments(this IEnumerable<KeyValuePair<int, ContentItem>> contentItems)
        {
            return string.Join("\r\n", contentItems
                .OrderBy(i => i.Key)
                .Select(j => j.Value.Id.ToString().AsComment("ContentItem"))
                .ToArray());
        }
    }
}
