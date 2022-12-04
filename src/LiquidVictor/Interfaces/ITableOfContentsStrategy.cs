using System;
using LiquidVictor.Entities;

namespace LiquidVictor.Interfaces
{
    public interface ITableOfContentsStrategy
    {
        TableOfContents GetTableOfContents(SlideDeck slideDeck);

        string GetMarkdown(SlideDeck slideDeck);
        string GetMarkdown(SlideDeck slideDeck, bool prettyPrint);

        Slide GetContentsSlide(SlideDeck slideDeck);
    }
}
