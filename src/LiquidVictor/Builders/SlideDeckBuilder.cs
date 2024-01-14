using System;
using System.Linq;

namespace LiquidVictor.Builders;

public class SlideDeckBuilder
{
    private readonly Entities.SlideDeck _slideDeck = new();

    private readonly SlidesBuilder _slidesBuilder = new();

    public Entities.SlideDeck Build()
    {
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

    public SlideDeckBuilder Slides(SlidesBuilder value)
    {
        value.ToList().ForEach(v => _slidesBuilder.Add(v.Key, v.Value));
        return this;
    }

    //public SlideDeckBuilder Slides(SlideBuilder value)
    //{
    //    _slidesBuilder.Add(value);
    //    return this;
    //}

}
