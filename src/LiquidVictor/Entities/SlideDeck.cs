using System;
using System.Collections.Generic;
using System.Linq;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Entities
{
    public class SlideDeck(Guid id, string title, string subTitle, string presenter,
         string themeName, Uri? slideDeckUrl, string printLinkText,
         Transition transition, AspectRatio aspectRatio, Format format,
         IOrderedEnumerable<IncludeBlock> includes)
    {
        const string _defaultThemeName = "Black";
        const AspectRatio _defaultAspectRatio = AspectRatio.Widescreen;
        const Transition _defaultTransition = Transition.Slide;
        const Transition _defaultBackgroundTransition = Transition.Fade;

        public SlideDeck()
            : this(Guid.NewGuid(), string.Empty, string.Empty, string.Empty, string.Empty, Array.Empty<IncludeBlock>().OrderBy(i => 0))
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, string printLinkText, IOrderedEnumerable<IncludeBlock> includes)
            : this(id, title, subTitle, presenter, _defaultThemeName, printLinkText, _defaultTransition, _defaultAspectRatio, includes)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, string themeName, string printLinkText, Transition transition, AspectRatio aspectRatio, IOrderedEnumerable<IncludeBlock> includes)
            : this(id, title, subTitle, presenter, themeName, null, printLinkText, transition, aspectRatio, includes)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter,
            string themeName, Uri? slideDeckUrl, string printLinkText,
            Transition transition, AspectRatio aspectRatio,
            IOrderedEnumerable<IncludeBlock> includes)
                : this(id, title, subTitle, presenter, themeName, slideDeckUrl, 
                      printLinkText, transition, aspectRatio, Format.Session, includes)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter,
         string themeName, string slideDeckUrl, string printLinkText,
         Transition transition, AspectRatio aspectRatio, Format format,
         IOrderedEnumerable<IncludeBlock> includes)
            : this(id, title, subTitle, presenter, themeName, new Uri(slideDeckUrl), printLinkText,
                  transition, aspectRatio, format, includes)
        { }


        public Guid Id { get; protected internal set; } = id;
        public string Title { get; set; } = title;
        public string SubTitle { get; set; } = subTitle;
        public string Presenter { get; set; } = presenter;
        public string ThemeName { get; set; } = themeName;

        public Uri? SlideDeckUrl { get; set; } = slideDeckUrl;
        
        public string PrintLinkText { get; set; } = printLinkText;
        public Transition Transition { get; set; } = transition;
        public Transition BackgroundTransition { get; set; } = _defaultBackgroundTransition;
        public AspectRatio AspectRatio { get; set; } = aspectRatio;
        public Format Format { get; set; } = format;

        internal IncludeBlockCollection Includes { get; } = new IncludeBlockCollection(includes);

        public ICollection<KeyValuePair<int, Slide>> Slides 
        { 
            get 
            {
                var i = 0;
                var result = new List<KeyValuePair<int, Slide>>();
                foreach (var includeBlock in this.Includes)
                    foreach (var slide in includeBlock.Slides)
                    {
                        result.Add(new KeyValuePair<int, Slide>(i, slide));
                        i++;
                    }
                return result;
            } 
        }

        public SlideDeck Clone(bool createNewId = true, bool createNewChildIds = false, string newTitle = "")
        {
            var id = createNewId ? Guid.NewGuid() : this.Id;
            var title = string.IsNullOrEmpty(newTitle) ? this.Title : newTitle;

            // Clone includes and produce an IOrderedEnumerable<IncludeBlock> preserving original order
            var includesClone = this.Includes
                .Select(i => i.Clone(createNewChildIds))
                .OrderBy(i => 0); // stable OrderBy with constant key preserves original order

            return new SlideDeck(id, title, this.SubTitle, this.Presenter, this.ThemeName, this.PrintLinkText, this.Transition, this.AspectRatio, includesClone)
            {
                BackgroundTransition = this.BackgroundTransition
            };
        }
    }
}
