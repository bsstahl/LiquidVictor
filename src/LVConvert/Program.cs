using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace LVConvert
{
    class Program
    {
        // Converts from one repository type to another

        static void Main(string[] args)
        {
            // TODO: Pull from args
            string outputPath = $"..\\..\\..\\..\\..\\..\\LiquidVictorDatabases\\JsonFileSystem\\IntroToWasmAndBlazor";
            var source = new LiquidVictor.Data.Postgres.SlideDeckReadRepository();
            var target = new LiquidVictor.Data.JsonFileSystem.SlideDeckWriteRepository(outputPath);

            Guid slideDeckId = Guid.Parse("c11b3e5f-1b2a-430c-8be7-b37377c4c198");
            var slideDeck = source.GetSlideDeck(slideDeckId);
            target.SaveSlideDeck(slideDeck);
        }

    }
}
