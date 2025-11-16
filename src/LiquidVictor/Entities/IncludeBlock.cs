using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Entities;

public class IncludeBlock
{
    private readonly List<Slide> _slides = new();

    public IOrderedEnumerable<Slide> Slides => _slides.OrderBy(s => _slides.IndexOf(s));

    public IncludeBlock()
    { }

    public IncludeBlock(Slide slide)
    {
        this.AddSlide(slide);
    }

    public IncludeBlock(IOrderedEnumerable<Slide> slides)
    {
        this.AddSlides(slides);
    }

    internal void AddSlide(Slide slide)
    {
        _slides.Add(slide);
    }

    internal void AddSlides(IOrderedEnumerable<Slide> slides)
    {
        _slides.AddRange(slides);
    }

    public IncludeBlock Clone(bool createNewChildIds = false)
    {
        var clone = new IncludeBlock();
        foreach (var slide in _slides)
            clone.AddSlide(slide.Clone(createNewId: createNewChildIds));
        return clone;
    }
}
