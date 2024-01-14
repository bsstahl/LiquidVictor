using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Builders;

public class SlideBuilder
{
    private readonly Entities.Slide _slide = new ();

    private readonly ContentItemsBuilder _contentItemsBuilder = new();

    private ContentItemBuilder _backgroundContentItemBuilder;

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

    public SlideBuilder BackgroundContent(ContentItemBuilder value)
    {
        _backgroundContentItemBuilder = value;
        return this;
    }

    public SlideBuilder BackgroundContent(Entities.ContentItem value)
    {
        // TODO: Convert to builder and call method above
        throw new NotImplementedException();
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

    public SlideBuilder ContentItems(ContentItemBuilder value)
    {
        var key = _contentItemsBuilder.Any()
            ? _contentItemsBuilder.Max(b => b.Key) + 1
            : 0;
        return this.ContentItems(key, value);
    }

    //public SlideBuilder ContentItems(IEnumerable<ContentItemBuilder> value)
    //{
    //    _contentItemsBuilder.AddRange(value);
    //    return this;
    //}

    //public SlideBuilder ContentItems(IEnumerable<Entities.ContentItem> value)
    //{
    //    // TODO: Convert to builders and call method above
    //    throw new NotImplementedException();
    //}

    public SlideBuilder Id(Guid value)
    {
        _slide.Id = value;
        return this;
    }

    public SlideBuilder Layout(Enumerations.Layout value)
    {
        _slide.Layout = value;
        return this;
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

    public SlideBuilder Title(string value)
    {
        _slide.Title = value;
        return this;
    }

    public SlideBuilder TransitionIn(Enumerations.Transition value)
    {
        _slide.TransitionIn = value;
        return this;
    }

    public SlideBuilder TransitionOut(Enumerations.Transition value)
    {
        _slide.TransitionOut = value;
        return this;
    }

}
