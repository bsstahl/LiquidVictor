using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using System;
using System.Linq;

namespace LiquidVictor.Builders;

public class SlideBuilder
{
    private readonly Entities.Slide _slide;

    private readonly ContentItemsBuilder _contentItemsBuilder = new();

    private ContentItemBuilder _backgroundContentItemBuilder;

    public SlideBuilder()
        : this(new Entities.Slide())
    { }

    public SlideBuilder(Slide value)
    {
        _slide = value;
        value?.ContentItems?.ToList().ForEach(ci => _ = this.ContentItems(ci.Key, ci.Value));
        _backgroundContentItemBuilder = new ContentItemBuilder(value.BackgroundContent);
    }

    public Entities.Slide Build()
    {
        return new Entities.Slide()
        {
            BackgroundContent = _backgroundContentItemBuilder?.Build(),
            ContentItems = _contentItemsBuilder.Build().ToList(),
            Id = _slide.Id,
            Layout = _slide.Layout,
            NeverFullScreen = _slide.NeverFullScreen,
            Notes = _slide.Notes,
            Title = _slide.Title,
            TransitionIn = _slide.TransitionIn,
            TransitionOut = _slide.TransitionOut
        };
    }

    public SlideBuilder Id(Guid value)
    {
        _slide.Id = value;
        return this;
    }

    public SlideBuilder Title(string value)
    {
        _slide.Title = value;
        return this;
    }

    public SlideBuilder Layout(Layout value)
    {
        _slide.Layout = value;
        return this;
    }

    public SlideBuilder Layout(string value)
    {
        return this.Layout((Layout)Enum.Parse(typeof(Layout), value));
    }

    public SlideBuilder TransitionIn(Transition value)
    {
        _slide.TransitionIn = value;
        return this;
    }

    public SlideBuilder TransitionIn(string value)
    {
        return this.TransitionIn((Transition)Enum.Parse(typeof(Transition), value));
    }

    public SlideBuilder TransitionOut(Transition value)
    {
        _slide.TransitionOut = value;
        return this;
    }

    public SlideBuilder TransitionOut(string value)
    {
        return this.TransitionOut((Transition)Enum.Parse(typeof(Transition), value));
    }

    public SlideBuilder BackgroundContent(ContentItemBuilder value)
    {
        _backgroundContentItemBuilder = value;
        return this;
    }

    public SlideBuilder BackgroundContent(Entities.ContentItem value)
    {
        return this.BackgroundContent(new ContentItemBuilder(value));
    }

    public SlideBuilder ContentItems(ContentItemsBuilder value)
    {
        value.ToList().ForEach(cib => _ = this.ContentItems(cib.Key, cib.Value));
        return this;
    }

    public SlideBuilder ContentItems(int key, ContentItemBuilder value)
    {
        _contentItemsBuilder.Add(key, value);
        return this;
    }

    public SlideBuilder ContentItems(ContentItem value)
    {
        var key = _contentItemsBuilder.Any()
            ? _contentItemsBuilder.Max(b => b.Key) + 1
            : 0;
        return this.ContentItems(key, value);
    }

    public SlideBuilder ContentItems(int key, ContentItem value)
    {
        return this.ContentItems(key, new ContentItemBuilder(value));
    }

    public SlideBuilder ContentItems(ContentItemBuilder value)
    {
        var key = _contentItemsBuilder.Any()
            ? _contentItemsBuilder.Max(b => b.Key) + 1
            : 0;
        return this.ContentItems(key, value);
    }

    public SlideBuilder NeverFullScreen(bool value)
    {
        _slide.NeverFullScreen = value;
        return this;
    }

    public SlideBuilder Notes(string value)
    {
        _slide.Notes = value;
        return this;
    }

}
