using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Builders;

public class SlideDecksBuilder
{
    private readonly List<SlideDeckBuilder> _slideDeckBuilders = new();

    public IEnumerable<Entities.SlideDeck> Build() 
        => _slideDeckBuilders.Select(sd => sd.Build());

    public SlideDecksBuilder Add(SlideDeckBuilder value)
    {
        _slideDeckBuilders.Add(value);
        return this;
    }

    public SlideDecksBuilder SlideDeck(SlideDeck value)
    {
        return value is null
            ? throw new ArgumentNullException(nameof(value))
            : this.Add(new SlideDeckBuilder(value));
    }
}
