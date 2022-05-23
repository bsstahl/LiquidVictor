using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Entities
{
    public class SlideDeck
    {
        const string _defaultThemeName = "Black";
        const AspectRatio _defaultAspectRatio = AspectRatio.Widescreen;
        const Transition _defaultTransition = Transition.Slide;

        public Guid Id { get; protected set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Presenter { get; set; }
        public string ThemeName { get; set; }
        public string SlideDeckUrl { get; set; }
        public string PrintLinkText { get; set; } = "Printable Version";
        public Transition Transition { get; set; } = Transition.Slide;

        public AspectRatio AspectRatio { get; set; }

        public ICollection<KeyValuePair<int, Slide>> Slides { get; protected set; }


        public SlideDeck()
            : this(Guid.NewGuid(), string.Empty, string.Empty, string.Empty, string.Empty, null)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, string printLinkText, ICollection<KeyValuePair<int, Slide>> slides)
            : this(id, title, subTitle, presenter, _defaultThemeName, printLinkText, _defaultTransition, _defaultAspectRatio, slides)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, string themeName, string printLinkText, Transition transition, AspectRatio aspectRatio, ICollection<KeyValuePair<int, Slide>> slides)
            : this(id, title, subTitle, presenter, themeName, string.Empty, printLinkText, transition, aspectRatio, slides)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, string themeName, 
            string slideDeckUrl, string printLinkText, Transition transition, AspectRatio aspectRatio, 
            ICollection<KeyValuePair<int, Slide>> slides)
        {
            this.Id = id;
            this.Title = title;
            this.SubTitle = subTitle;
            this.Presenter = presenter;
            this.ThemeName = themeName;
            this.SlideDeckUrl = slideDeckUrl;
            this.PrintLinkText = printLinkText;
            this.Transition = transition;
            this.AspectRatio = aspectRatio;
            this.Slides = slides;
        }

        public SlideDeck Clone(Guid? newId = null, string newTitle = "")
        {
            var id = newId ?? this.Id;
            var title = string.IsNullOrEmpty(newTitle) ? this.Title : newTitle;
            return new SlideDeck(id, title, this.SubTitle, this.Presenter, this.ThemeName, this.PrintLinkText, this.Transition, this.AspectRatio, this.Slides);
        }
    }
}
