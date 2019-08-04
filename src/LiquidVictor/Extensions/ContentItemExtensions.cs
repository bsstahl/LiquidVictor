using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Extensions
{
    public static class ContentItemExtensions
    {
        public static ICollection<KeyValuePair<int, ContentItem>> Clone(this IEnumerable<KeyValuePair<int, ContentItem>> contentItemPairs)
        {
            var result = new List<KeyValuePair<int, ContentItem>>();
            foreach (var pair in contentItemPairs)
            {
                result.Add(new KeyValuePair<int, ContentItem>(pair.Key, pair.Value));
            }
            return result;
        }
    }
}
