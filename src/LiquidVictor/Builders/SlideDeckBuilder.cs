using LiquidVictor.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Builders;

public class SlideDeckBuilder
{
    private readonly Entities.SlideDeck _slideDeck;

    private readonly SlidesBuilder _slidesBuilder = [];

    public SlideDeckBuilder()
        : this(new Entities.SlideDeck())
    { }

    public SlideDeckBuilder(Entities.SlideDeck value)
    {
        _slideDeck = value;
        value?.Slides.ToList().ForEach(s => _slidesBuilder.Add(s.Value));
    }

    public Entities.SlideDeck Build()
    {
        _slideDeck.Id = _slideDeck.Id.Equals(Guid.Empty) 
            ? Guid.NewGuid() 
            : _slideDeck.Id;
        _slidesBuilder.Build().ToList().ForEach(s => _slideDeck.Slides.Add(s));
        return _slideDeck;
    }

    public SlideDeckBuilder Id(string value) => this.Id(Guid.Parse(value));
    public SlideDeckBuilder Id(Guid value)
    {
        _slideDeck.Id = value;
        return this;
    }

    public SlideDeckBuilder Title(string value)
    {
        _slideDeck.Title = value;
        return this;
    }

    public SlideDeckBuilder SubTitle(string value)
    {
        _slideDeck.SubTitle = value;
        return this;
    }

    public SlideDeckBuilder Presenter(string value)
    {
        _slideDeck.Presenter = value;
        return this;
    }

    public SlideDeckBuilder PrintLinkText(string value)
    {
        _slideDeck.PrintLinkText = value;
        return this;
    }

    public SlideDeckBuilder ThemeName(string value)
    {
        _slideDeck.ThemeName = value;
        return this;
    }

    public SlideDeckBuilder AspectRatio(string value)
    {
        AspectRatio aspectRatio = (AspectRatio)Enum.Parse(typeof(AspectRatio), value);
        return this.AspectRatio(aspectRatio);
    }

    public SlideDeckBuilder AspectRatio(AspectRatio value)
    {
        _slideDeck.AspectRatio = value;
        return this;
    }

    public SlideDeckBuilder Transition(string value)
    {
        var transition = (Transition)Enum.Parse(typeof(Transition), value);
        return this.Transition(transition);
    }

    public SlideDeckBuilder Transition(Transition value)
    {
        _slideDeck.Transition = value;
        return this;
    }

    public SlideDeckBuilder SlideDeckUrl(string value)
    {
        _slideDeck.SlideDeckUrl = new Uri(value);
        return this;
    }

    public SlideDeckBuilder Slides(SlidesBuilder value)
    {
        value.ToList().ForEach(v => _slidesBuilder.Add(v.Key, v.Value));
        return this;
    }

    public SlideDeckBuilder Slides(IEnumerable<Entities.Slide> value)
    {
        return value is null
            ? throw new ArgumentNullException(nameof(value))
            : this.Slides(new SlidesBuilder().Add(value));
    }
}
