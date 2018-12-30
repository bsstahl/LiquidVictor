using Microsoft.Extensions.Configuration;
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
            // TODO: Pull from args
            string outputPath = $"..\\..\\..\\..\\..\\Presentations\\{Guid.NewGuid().ToString()}\\";
            var source = new LiquidVictor.Data.Postgres.SlideDeckReadRepository();

            Guid slideDeckId = Guid.Parse("E0B187D2-C9B7-4635-8FE5-0CA21BC5007F");
            var slideDeck = source.GetSlideDeck(slideDeckId);

            var engine = new LiquidVictor.Output.RevealJs.Generator.Engine();
            engine.CreatePresentation(outputPath, slideDeck);
        }

    }
}
