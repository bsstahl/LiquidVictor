using LiquidVictor.Entities;

namespace LiquidVictor.Interfaces
{
    public interface IPresentationBuilder
    {
        void CreatePresentation(string filepath, SlideDeck slideDeck, Configuration configuration);
        void CompilePresentation(SlideDeck slideDeck, Configuration configuration);
    }
}