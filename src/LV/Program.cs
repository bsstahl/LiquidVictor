using LiquidVictor.Interfaces;
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
            // TODO: Convert to using connection strings for source and target
            // LV.exe [SourceRepoType] [SourceRepoLocation] [SlideDeckId] [TargetType] [TargetTemplateLocation] [TargetLocation]

            //Console.WriteLine();
            //for (int i = 0; i < args.Length; i++)
            //{
            //    Console.WriteLine($"{i}: {args[i]}");
            //}
            //Console.WriteLine();

            ISlideDeckReadRepository source = GetSourceRepository(args[0], args[1]);

            string outputPath = System.IO.Path.GetFullPath(args[5]);
            IPresentationBuilder engine = GetEngine(args[3], args[4]);

            Guid slideDeckId = Guid.Parse(args[2]);
            var slideDeck = source.GetSlideDeck(slideDeckId);

            var config = new LiquidVictor.Entities.Configuration();
            for (int i = 6; i < args.Length; i++)
            {
                // TODO: Add any other arguments
                string arg = args[i].ToLower();
                if (arg == "--notitle")
                    config.BuildTitleSlide = false;
                else if (arg == "--makesoloimagesfullscreen")
                    config.MakeSoloImagesFullScreen = true;
            }

            engine.CreatePresentation(outputPath, slideDeck, config);

            Console.WriteLine($"Presentation written to {outputPath}");
        }

        private static IPresentationBuilder GetEngine(string engineType, string engineParameters)
        {
            IPresentationBuilder result = null;
            switch (engineType.ToLower())
            {
                case "reveal":
                case "revealjs":
                    result = new LiquidVictor.Output.RevealJs.Generator.Engine(engineParameters);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(engineType), $"Invalid Presentation Builder '{engineType};");
            }

            return result;
        }

        private static ISlideDeckReadRepository GetSourceRepository(string repoType, string repoParameter)
        {
            ISlideDeckReadRepository result = null;
            switch (repoType.ToLower())
            {
                case "postgres":
                case "postgresql":
                    result = new LiquidVictor.Data.Postgres.SlideDeckReadRepository();
                    break;
                default:
                    result = new LiquidVictor.Data.JsonFileSystem.SlideDeckReadRepository(repoParameter);
                    break;
            }

            return result;
        }
    }
}
