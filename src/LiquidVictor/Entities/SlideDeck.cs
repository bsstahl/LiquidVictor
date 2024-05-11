using System;
using System.Collections.Generic;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;

namespace LiquidVictor.Entities
{
    public class SlideDeck(Guid id, string title, string subTitle, string presenter,
         string themeName, Uri? slideDeckUrl, string printLinkText,
         Transition transition, AspectRatio aspectRatio, Format format,
         ICollection<KeyValuePair<int, Slide>> slides)
    {
        const string _defaultThemeName = "Black";
        const AspectRatio _defaultAspectRatio = AspectRatio.Widescreen;
        const Transition _defaultTransition = Transition.Slide;

        public SlideDeck()
            : this(Guid.NewGuid(), string.Empty, string.Empty, string.Empty, string.Empty, [])
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, string printLinkText, ICollection<KeyValuePair<int, Slide>> slides)
            : this(id, title, subTitle, presenter, _defaultThemeName, printLinkText, _defaultTransition, _defaultAspectRatio, slides)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, string themeName, string printLinkText, Transition transition, AspectRatio aspectRatio, ICollection<KeyValuePair<int, Slide>> slides)
            : this(id, title, subTitle, presenter, themeName, null, printLinkText, transition, aspectRatio, slides)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter,
            string themeName, Uri? slideDeckUrl, string printLinkText,
            Transition transition, AspectRatio aspectRatio,
            ICollection<KeyValuePair<int, Slide>> slides)
                : this(id, title, subTitle, presenter, themeName, slideDeckUrl, 
                      printLinkText, transition, aspectRatio, Format.Session, slides)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter,
         string themeName, string slideDeckUrl, string printLinkText,
         Transition transition, AspectRatio aspectRatio, Format format,
         ICollection<KeyValuePair<int, Slide>> slides)
            : this(id, title, subTitle, presenter, themeName, new Uri(slideDeckUrl), printLinkText,
                  transition, aspectRatio, format, slides)
        { }


        public Guid Id { get; protected internal set; } = id;
        public string Title { get; set; } = title;
        public string SubTitle { get; set; } = subTitle;
        public string Presenter { get; set; } = presenter;
        public string ThemeName { get; set; } = themeName;

        public Uri? SlideDeckUrl { get; set; } = slideDeckUrl;
        
        public string PrintLinkText { get; set; } = printLinkText;
        public Transition Transition { get; set; } = transition;
        public AspectRatio AspectRatio { get; set; } = aspectRatio;
        public Format Format { get; set; } = format;

        public ICollection<KeyValuePair<int, Slide>> Slides { get; } = slides;



        public SlideDeck Clone(bool createNewId = true, bool createNewChildIds = false, string newTitle = "")
        {
            var id = createNewId ? Guid.NewGuid() : this.Id;
            var title = string.IsNullOrEmpty(newTitle) ? this.Title : newTitle;
            return new SlideDeck(id, title, this.SubTitle, this.Presenter, this.ThemeName, this.PrintLinkText, this.Transition, this.AspectRatio, this.Slides.Clone(createNewChildIds));
        }
    }
}
