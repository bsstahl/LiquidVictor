using System;
using System.Collections.Generic;
using System.Text;
using LiquidVictor.Entities;

namespace LiquidVictor.Output.RevealJs.LayoutStrategy
{
    internal class FullPage : ILayoutStrategy
    {
        public string Layout(SlideDeck deck, Slide slide)
        {
            return $"<section><h1>{slide.Title}</h1>{Markdig.Markdown.ToHtml(slide.ContentText)}</section>\r\n";
        }
    }
}
