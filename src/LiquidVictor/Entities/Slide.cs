using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using System;
using System.Collections.Generic;

namespace LiquidVictor.Entities;

public class Slide
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public Layout Layout { get; set; }

    public Transition TransitionIn { get; set; }
    public Transition TransitionOut { get; set; }

    public string Notes { get; set; } = string.Empty;

    public ContentItem? BackgroundContent { get; set; }
    public bool NeverFullScreen { get; set; }

    public ICollection<KeyValuePair<int, ContentItem>> ContentItems { get; internal set; } = [];

    public Slide()
    { }

    public Slide(Guid id, string title, Layout layout, Transition transitionIn, Transition transitionOut, string notes, ContentItem? backgroundContent, bool neverFullScreen, ICollection<KeyValuePair<int, ContentItem>> contentItems)
    {
        Id = id;
        Title = title;
        Layout = layout;
        TransitionIn = transitionIn;
        TransitionOut = transitionOut;
        Notes = notes;
        BackgroundContent = backgroundContent;
        NeverFullScreen = neverFullScreen;
        ContentItems = contentItems;
    }

    public Slide Clone(bool createNewId = false)
    {
        return new Slide()
        {
            Id = createNewId ? Guid.NewGuid() : this.Id,
            Title = this.Title,
            Layout = this.Layout,
            TransitionIn = this.TransitionIn,
            TransitionOut = this.TransitionOut,
            Notes = this.Notes,
            BackgroundContent = this.BackgroundContent?.Clone(createNewId),
            NeverFullScreen = this.NeverFullScreen,
            ContentItems = this.ContentItems?.Clone(createNewId) ?? []
        };
    }
}
