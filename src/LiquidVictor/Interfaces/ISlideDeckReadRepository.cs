using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Interfaces
{
    public interface ISlideDeckReadRepository
    {

        SlideDeck GetSlideDeck(Guid id);
        IncludeBlock GetIncludeBlock(Guid id);
        Slide GetSlide(Guid id);
        ContentItem GetContentItem(Guid id);

        IEnumerable<Guid> GetSlideDeckIds();
        IEnumerable<Guid> GetSlideIds();
        IEnumerable<Guid> GetContentItemIds();
        IEnumerable<Guid> GetIncludeBlockIds();

        IEnumerable<SlideDeck> GetSlideDecks();
        IEnumerable<Slide> GetSlides();
        IEnumerable<ContentItem> GetContentItems();
        IEnumerable<IncludeBlock> GetIncludeBlocks();
    }
}
