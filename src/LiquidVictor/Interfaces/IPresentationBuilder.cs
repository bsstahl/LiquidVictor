using LiquidVictor.Entities;

namespace LiquidVictor.Interfaces
{
    public interface IPresentationBuilder
    {
        void CreatePresentation(string filepath, SlideDeck slideDeck, bool buildTitleSlide);
    }
}