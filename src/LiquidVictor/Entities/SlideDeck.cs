using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiquidVictor.Entities
{
    public class SlideDeck
    {
        public Guid Id { get; protected set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Presenter { get; set; }

        // TODO: Add AspectRatio (16:9 or 4:3)

        public IOrderedEnumerable<KeyValuePair<int, Slide>> Slides { get; protected set; }

        public SlideDeck()
        {
            this.Id = Guid.NewGuid();
        }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, IOrderedEnumerable<KeyValuePair<int, Slide>> slides)
        {
            this.Id = id;
            this.Title = title;
            this.SubTitle = subTitle;
            this.Presenter = presenter;
            this.Slides = slides;
        }
    }
}
