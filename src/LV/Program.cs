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
            IPresentationBuilder engine = GetEngine(args[3], args[4]);

            var slideDeck = source.GetSlideDeck(Guid.Parse(args[2]));
            var (command, config) = args.Parse();

            if (command == Command.Build)
            {
                string outputPath = System.IO.Path.GetFullPath(args[5]);
                if (config.SkipOutput)
                {
                    engine.CompilePresentation(slideDeck, config);
                    Console.WriteLine($"Presentation '{slideDeck.Title}' successfully compiled");
                }
                else
                {
                    engine.CreatePresentation(outputPath, slideDeck, config);
                    Console.WriteLine($"Presentation '{slideDeck.Title}' written to {outputPath}");
                }
            }
            else
                throw new NotImplementedException($"The '({command})' feature has not yet been implemented");

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
