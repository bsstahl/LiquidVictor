using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Interfaces
{
    public interface ISlideDeckReadRepository
    {
        SlideDeck GetSlideDeck(Guid id);
        Slide GetSlide(Guid id);
        ContentItem GetContentItem(Guid id);
    }
}
