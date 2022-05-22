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

            var (command, config) = args.Parse();

            var source = GetSourceRepository(args[0], args[1]);
            var target = GetTargetRepository(args[0], args[1]); // TODO: Is args[1] the right parameter?
            var engine = GetEngine(args[3], args[4], config);


            switch (command)
            {
                case Command.Build:
                    var slideDeck = source.GetSlideDeck(config.SlideDeckId);
                    if (config.SkipOutput)
                    {
                        engine.CompilePresentation(slideDeck);
                        Console.WriteLine($"Presentation '{slideDeck.Title}' successfully compiled");
                    }
                    else
                    {
                        engine.CreatePresentation(config.OutputPath, slideDeck);
                        Console.WriteLine($"Presentation '{slideDeck.Title}' written to {config.OutputPath}");
                    }
                    break;

                case Command.CloneSlide:
                    // TODO: Validate inputs
                    var slide = source.GetSlide(config.SlideId);
                    
                    var newSlide = slide.Clone();
                    newSlide.Id = Guid.NewGuid();

                    target.SaveSlide(newSlide);
                    Console.WriteLine($"Slide {newSlide.Id} ('{newSlide.Title}') written to {config.OutputPath}");
                    break;

                default:
                    throw new NotImplementedException($"The '({command})' feature has not yet been implemented");
            }
        }

        private static IPresentationBuilder GetEngine(string engineType, string engineParameters, Configuration config)
        {
            IPresentationBuilder result = null;
            switch (engineType.ToLower())
            {
                case "reveal":
                case "revealjs":
                    var builderOptions = new LiquidVictor.Output.RevealJs.Entities.BuilderOptions()
                    {
                        BuildTitleSlide = config.BuildTitleSlide,
                        MakeSoloImagesFullScreen = config.MakeSoloImagesFullScreen
                    };
                    result = new LiquidVictor.Output.RevealJs.Generator.Engine(engineParameters, builderOptions);
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

        private static ISlideDeckWriteRepository GetTargetRepository(string repoType, string repoParameter)
        {
            ISlideDeckWriteRepository result = null;
            switch (repoType.ToLower())
            {
                case "postgres":
                case "postgresql":
                    result = new LiquidVictor.Data.Postgres.SlideDeckWriteRepository(repoParameter);
                    break;
                default:
                    result = new LiquidVictor.Data.JsonFileSystem.SlideDeckWriteRepository(repoParameter);
                    break;
            }

            return result;
        }
    }
}
