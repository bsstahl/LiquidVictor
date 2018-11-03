using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Entities
{
    public class SlideDeck
    {
        public Guid Id { get; protected set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public SortedList<int, Slide> Slides { get; protected set; }

        public SlideDeck()
        {
            this.Id = Guid.NewGuid();
        }

        public SlideDeck(Guid id, string title, string subTitle, SortedList<int, Slide> slides)
        {
            this.Id = id;
            this.Title = title;
            this.SubTitle = subTitle;
            this.Slides = slides;
        }
    }
}
