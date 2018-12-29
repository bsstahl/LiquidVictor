using LiquidVictor.Entities;

namespace LiquidVictor.Interfaces
{
    public interface ISlideDeckWriteRepository
    {
        void SaveSlideDeck(Entities.SlideDeck slideDeck);
    }
}