using LiquidVictor.Entities;
using LiquidVictor.Interfaces;

namespace LiquidVictor.Output.Hardcoded;

public class Engine : IPresentationBuilder
{
    public SlideDeck SlideDeck { get; private set; }

    public void CompilePresentation(SlideDeck slideDeck)
    {
        this.SlideDeck = slideDeck;
    }

    public void CreatePresentation(String filepath, SlideDeck slideDeck) => throw new NotImplementedException();
}
