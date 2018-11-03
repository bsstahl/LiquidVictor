using System;

namespace LV
{
    class Program
    {
        // This is the 1st pass at a prototype that will load
        // a slide deck from a pre-defined (hardcoded) source repository
        // and turn it into a Powerpoint presentation

        static void Main(string[] args)
        {
            string filepath = @"..\..\..\..\..\Presentations\";
            string filename = $"{Guid.NewGuid().ToString()}.pptx";
            string fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(filepath, filename));

            Guid slideDeckId = Guid.NewGuid();

            var source = new LiquidVictor.Data.Hardcoded.SlideDeckRepository();
            var slideDeck = source.GetSlideDeck(slideDeckId);

            var engine = new LiquidVictor.Output.Powerpoint.Engine();
            engine.CreatePresentation(fullPath, slideDeck);
        }

    }
}
