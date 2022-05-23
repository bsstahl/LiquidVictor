using LiquidVictor.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LV
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Add configurable default values for all config items

            var (command, config) = args.Parse();

            var readRepo = GetReadRepository(config);
            var writeRepo = GetWriteRepository(config);
            var engine = GetEngine(config);

            switch (command)
            {
                case Command.CreateSlideDeck:
                    ExecuteCreateSlideDeck(config, writeRepo);
                    break;

                case Command.Build:
                    ExecuteBuild(config, readRepo, engine);
                    break;

                case Command.CloneSlide:
                    ExecuteCloneSlide(config, readRepo, writeRepo);
                    break;

                default:
                    throw new NotImplementedException($"The '{command}' feature has not yet been implemented");
            }
        }

        private static void ExecuteCreateSlideDeck(Configuration config, ISlideDeckWriteRepository writeRepo)
        {
            // TODO: Respect --SkipOutput switch
            var slideDeck = new LiquidVictor.Entities
                .SlideDeck(Guid.NewGuid(), config.Title, string.Empty, string.Empty, string.Empty, new List<KeyValuePair<int, LiquidVictor.Entities.Slide>>());
            writeRepo.SaveSlideDeck(slideDeck);
            Console.WriteLine($"Slide Deck {slideDeck.Id} ('{slideDeck.Title}') written to {config.SourceRepoPath}");
        }

        private static void ExecuteCloneSlide(Configuration config, ISlideDeckReadRepository readRepo, ISlideDeckWriteRepository writeRepo)
        {
            // TODO: Validate inputs
            // TODO: Respect --SkipOutput switch
            var slide = readRepo.GetSlide(config.SlideId);

            var newSlide = slide.Clone();
            newSlide.Id = Guid.NewGuid();

            writeRepo.SaveSlide(newSlide);
            Console.WriteLine($"Slide {newSlide.Id} ('{newSlide.Title}') written to {config.SourceRepoPath}");
        }

        private static void ExecuteBuild(Configuration config, ISlideDeckReadRepository readRepo, IPresentationBuilder engine)
        {
            // Load a slide deck from a source repository
            // and build it into a RevealJS presentation

            // TODO: Validate Inputs
            var slideDeck = readRepo.GetSlideDeck(config.SlideDeckId);
            if (config.SkipOutput)
            {
                engine.CompilePresentation(slideDeck);
                Console.WriteLine($"Presentation '{slideDeck.Title}' successfully compiled");
            }
            else
            {
                engine.CreatePresentation(config.PresentationPath, slideDeck);
                Console.WriteLine($"Presentation '{slideDeck.Title}' written to {config.PresentationPath}");
            }
        }

        private static IPresentationBuilder GetEngine(Configuration config)
        {
            IPresentationBuilder result = null;
            switch (config.OutputEngineType.ToLower())
            {
                case "reveal":
                case "revealjs":
                    var builderOptions = new LiquidVictor.Output.RevealJs.Entities.BuilderOptions()
                    {
                        BuildTitleSlide = config.BuildTitleSlide,
                        MakeSoloImagesFullScreen = config.MakeSoloImagesFullScreen
                    };
                    result = new LiquidVictor.Output.RevealJs.Generator.Engine(config.TemplatePath, builderOptions);
                    break;
                default:
                    throw new NotSupportedException($"Invalid Presentation Builder '{config.OutputEngineType};");
            }

            return result;
        }

        private static ISlideDeckReadRepository GetReadRepository(Configuration config)
        {
            ISlideDeckReadRepository result = null;
            switch (config.SourceRepoType.ToLower())
            {
                case "postgres":
                case "postgresql":
                    result = new LiquidVictor.Data.Postgres.SlideDeckReadRepository();
                    break;
                default:
                    result = new LiquidVictor.Data.JsonFileSystem.SlideDeckReadRepository(config.SourceRepoPath);
                    break;
            }

            return result;
        }

        private static ISlideDeckWriteRepository GetWriteRepository(Configuration config)
        {
            ISlideDeckWriteRepository result = null;
            switch (config.SourceRepoType.ToLower())
            {
                case "postgres":
                case "postgresql":
                    result = new LiquidVictor.Data.Postgres.SlideDeckWriteRepository(config.SourceRepoPath);
                    break;
                default:
                    result = new LiquidVictor.Data.JsonFileSystem.SlideDeckWriteRepository(config.SourceRepoPath);
                    break;
            }

            return result;
        }
    }
}
