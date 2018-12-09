using System;
using System.Linq;

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

            // EF Postgres Repository
            Guid slideDeckId = Guid.Parse("	c11b3e5f-1b2a-430c-8be7-b37377c4c198");
            var source = new LiquidVictor.Data.Postgres.ReadRepository();

            // JsonFile Repository
            //Guid slideDeckId = Guid.Parse("C11B3E5F-1B2A-430C-8BE7-B37377C4C198");    // WASM Using Blazor presentation
            //var source = new LiquidVictor.Data.JsonFile.SlideDeckRepository("..\\..\\..\\..\\..\\SlideDeckRepository.json");

            // Hardcoded Repository
            // Guid slideDeckId = Guid.Parse("FF9BE76C-EB21-447F-A41F-76383259A454"); // bad Id
            //Guid slideDeckId = Guid.Parse("E0B187D2-C9B7-4635-8FE5-0CA21BC5007F");    // Demo presentation
            //var source = new LiquidVictor.Data.Hardcoded.SlideDeckRepository();

            var slideDeck = source.GetSlideDeck(slideDeckId);

            var engine = new LiquidVictor.Output.RevealJs.Engine();
            engine.CreatePresentation(outputPath, slideDeck);

            //var target = new LiquidVictor.Data.Postgres.SlideDeckWriteRepository();
            //target.SaveSlideDeck(slideDeck);
        }

    }
}
