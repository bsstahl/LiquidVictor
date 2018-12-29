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

            var config = new ConfigurationBuilder()
                .AddUserSecrets<Config>()
                .Build();

            // EF Postgres Repository
            string db = config["Db"];
            var source = new LiquidVictor.Data.Postgres.SlideDeckReadRepository(db);

            Guid slideDeckId = Guid.Parse("c11b3e5f-1b2a-430c-8be7-b37377c4c198");
            var slideDeck = source.GetSlideDeck(slideDeckId);

            var engine = new LiquidVictor.Output.RevealJs.Engine();
            engine.CreatePresentation(outputPath, slideDeck);
        }

    }
}
