using LiquidVictor.Entities;

namespace LiquidVictor.Interfaces
{
    public interface ISlideDeckWriteRepository
    {
        void SaveSlideDeck(Entities.SlideDeck slideDeck);
        void SaveSlide(Entities.Slide slide);
        void SaveContentItem(Entities.ContentItem contentItem);
    }
}