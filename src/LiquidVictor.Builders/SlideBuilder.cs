using System;
using System.Collections.Generic;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Builders
{
    public class SlideBuilder: Slide
    {

        public Slide Build()
        {
            return this;
        }

        public new SlideBuilder Id(Guid id)
        {
            base.Id = id;
            return this;
        }

        public SlideBuilder AssignId()
        {
            return this.Id(Guid.NewGuid());
        }

        public new SlideBuilder Layout(Enumerations.Layout layout)
        {
            base.Layout = layout;
            return this;
        }

        public new SlideBuilder Notes(string notes)
        {
            base.Notes = notes;
            return this;
        }

        public new SlideBuilder Title(string title)
        {
            base.Title = title;
            return this;
        }

        public new SlideBuilder TransitionIn(Enumerations.Transition transition)
        {
            base.TransitionIn = transition;
            return this;
        }

        public new SlideBuilder TransitionOut(Enumerations.Transition transition)
        {
            base.TransitionOut = transition;
            return this;
        }

        public SlideBuilder WithEmptyContentItemsCollection()
        {
            base.ContentItems = new List<KeyValuePair<int, ContentItem>>();
            return this;
        }

        public SlideBuilder AddContentItem(int key, ContentItem item)
        {
            if (base.ContentItems == null)
                base.ContentItems = new List<KeyValuePair<int, ContentItem>>();
            base.ContentItems.Add(new KeyValuePair<int, ContentItem>(key, item));
            return this;
        }
    }
}
