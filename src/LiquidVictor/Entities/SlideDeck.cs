﻿using System;
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
        public Transition Transition { get; set; } = Transition.Slide;

        public AspectRatio AspectRatio { get; set; }

        public ICollection<KeyValuePair<int, Slide>> Slides { get; protected set; }


        public SlideDeck()
            : this(Guid.NewGuid(), string.Empty, string.Empty, string.Empty, null)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, ICollection<KeyValuePair<int, Slide>> slides)
            : this(id, title, subTitle, presenter, _defaultThemeName, _defaultTransition, _defaultAspectRatio, slides)
        { }

        public SlideDeck(Guid id, string title, string subTitle, string presenter, string themeName, Transition transition, AspectRatio aspectRatio, ICollection<KeyValuePair<int, Slide>> slides)
        {
            this.Id = id;
            this.Title = title;
            this.SubTitle = subTitle;
            this.Presenter = presenter;
            this.ThemeName = themeName;
            this.Transition = transition;
            this.AspectRatio = aspectRatio;
            this.Slides = slides;
        }
    }
}
