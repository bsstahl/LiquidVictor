using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Builders;

public class SlideDecksBuilder
{
    private readonly List<SlideDeckBuilder> _slideDeckBuilders = new();

    public IEnumerable<Entities.SlideDeck> Build() 
        => _slideDeckBuilders.Select(sd => sd.Build());

    public SlideDecksBuilder SlideDeck(SlideDeckBuilder value)
    {
        _slideDeckBuilders.Add(value);
        return this;
    }
}
