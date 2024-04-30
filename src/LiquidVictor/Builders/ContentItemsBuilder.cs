using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Builders;

public class ContentItemsBuilder:Dictionary<int, ContentItemBuilder>
{
    public IEnumerable<KeyValuePair<int, Entities.ContentItem>> Build()
        => this.Select(t => new KeyValuePair<int, Entities.ContentItem>(t.Key, t.Value.Build()));

    public ContentItemsBuilder Add(ContentItemBuilder value)
    {
        this.Add(this.Any() ? this.Max(b => b.Key) + 1 : 0, value);
        return this;
    }

}
