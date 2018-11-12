using System;

namespace LV
{
    class Program
    {
        // This is the 1st pass at a prototype that will load
        // a slide deck from a pre-defined (hardcoded) source repository
        // and turn it into a RevealJS presentation

        static void Main(string[] args)
        {
            string outputPath = $"..\\..\\..\\..\\..\\Presentations\\{Guid.NewGuid().ToString()}\\";

            Guid slideDeckId = Guid.NewGuid();

            var source = new LiquidVictor.Data.Hardcoded.SlideDeckRepository();
            var slideDeck = source.GetSlideDeck(slideDeckId);

            var engine = new LiquidVictor.Output.RevealJs.Engine();
            engine.CreatePresentation(outputPath, slideDeck);
        }

    }
}
