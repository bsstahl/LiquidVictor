using LiquidVictor.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Data.JsonFileSystem
{
    internal class SlideDeck
    {
        const Transition _defaultTransition = Enumerations.Transition.Slide;

        public string Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Presenter { get; set; }
        public string ThemeName { get; set; }
        public string AspectRatio { get; set; }
        public string PrintLinkText { get; set; }
        public string Transition { get; set; }
        public string SlideDeckUrl { get; set; }
        public string[] SlideIds { get; set; }

        internal Transition GetTransition()
        {
            Transition result = _defaultTransition;
            Enum.TryParse<Enumerations.Transition>(this.Transition, out result);
            return result;
        }
    }

}
