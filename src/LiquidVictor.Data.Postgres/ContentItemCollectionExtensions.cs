using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Data.Postgres
{
    public static class ContentItemCollectionExtensions
    {
        internal static ICollection<KeyValuePair<int, Entities.ContentItem>> AsEntities(this IEnumerable<SlideContentItem> slideContentItems)
        {
            var result = new List<KeyValuePair<int, Entities.ContentItem>>();
            foreach (var slideContentItem in slideContentItems)
                result.Add(new KeyValuePair<int, Entities.ContentItem>(slideContentItem.SortOrder, slideContentItem.ContentItem.AsEntity()));
            return result;
        }
    }
}
