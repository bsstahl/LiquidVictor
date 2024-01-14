using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Builders;

public class SlidesBuilder : Dictionary<int, SlideBuilder>
{
    public IEnumerable<Entities.Slide> BuildUnorderedList()
        => this.Select(s => s.Value.Build());

    public IEnumerable<KeyValuePair<int, Entities.Slide>> Build()
        => this.Select(t => new KeyValuePair<int, Entities.Slide>(t.Key, t.Value.Build()));

    public SlidesBuilder Add(SlideBuilder value)
    {
        this.Add(this.Any() ? this.Max(b => b.Key) + 1 : 0, value);
        return this;
    }

}
