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
            const string defaultConfigPath = @"defaults.json";

            Command command;
            Configuration config;

            string executionFolder = System.IO.Path.GetDirectoryName(AppContext.BaseDirectory);
            string fullConfigPath = System.IO.Path.Combine(executionFolder, defaultConfigPath);
            if (System.IO.File.Exists(fullConfigPath))
            {
                var defaults = new ConfigurationBuilder()
                    .AddJsonFile(defaultConfigPath, false)
                    .Build();
                (command, config) = args.Parse(defaults);
            }
            else
                (command, config) = args.Parse();

            var readRepo = GetReadRepository(config);
            var writeRepo = GetWriteRepository(config);
            var engine = GetEngine(config);

            try
            {
                switch (command)
                {
                    case Command.Help: // TODO: Implement Help command
                        throw new NotImplementedException("Help command not yet implemented");

                    case Command.Build:
                        ExecuteBuild(config, readRepo, engine);
                        break;

                    case Command.CreateSlideDeck:
                        ExecuteCreateSlideDeck(config, writeRepo);
                        break;

                    case Command.CreateSlide:
                        ExecuteCreateSlide(config, writeRepo);
                        break;

                    case Command.CreateContentItem:
                        ExecuteCreateContentItem(config, writeRepo);
                        break;

                    case Command.ExportContentItem: // TODO: Implement ExportContentItem command
                        throw new NotImplementedException("ExportContentItem command not yet implemented");

                    case Command.CloneSlideDeck:
                        ExecuteCloneSlideDeck(config, readRepo, writeRepo);
                        break;

                    case Command.CloneSlide:
                        ExecuteCloneSlide(config, readRepo, writeRepo);
                        break;

                    case Command.CloneContentItem: // TODO: Implement CloneContentItem command
                        throw new NotImplementedException("CloneContentItem command not yet implemented");

                    case Command.ValidateSourceRepo: // TODO: Implement ValidateSourceRepo command
                        // Verifies that all SlideDecks have unique IDs
                        throw new NotImplementedException();

                    case Command.FindOrphans: // TODO: Implement FindOrphans command
                        throw new NotImplementedException();

                    default:
                        throw new NotImplementedException($"The '{command}' feature has not yet been implemented");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }        
        }

        private static void ExecuteCreateSlide(Configuration config, ISlideDeckWriteRepository writeRepo)
        {
            // TODO: Validate inputs
            // TODO: Respect --SkipOutput switch
            // TODO: Create ContentItem as background content

            var contentItem = new LiquidVictor.Entities.ContentItem()
            {
                Id = Guid.NewGuid(),
                ContentType = GetContentType(config.ContentPath),
                Title = config.Title,
                FileName = System.IO.Path.GetFileName(config.ContentPath),
                Content = System.IO.File.ReadAllBytes(config.ContentPath)
            };

            var contentItems = new List<KeyValuePair<int, LiquidVictor.Entities.ContentItem>>()
            {
                new KeyValuePair<int,LiquidVictor.Entities.ContentItem>(0, contentItem)
            };

            var slide = new LiquidVictor.Entities.Slide()
            {
                Id = Guid.NewGuid(),
                BackgroundContent  = null,
                ContentItems = contentItems,
                Layout = LiquidVictor.Enumerations.Layout.FullPage,
                NeverFullScreen = false,
                Notes = null,
                Title = config.Title,
                TransitionIn = LiquidVictor.Enumerations.Transition.PresentationDefault,
                TransitionOut = LiquidVictor.Enumerations.Transition.PresentationDefault
            };

            writeRepo.SaveSlide(slide);
            Console.WriteLine($"Slide {slide.Id} ('{slide.Title}') written to {config.SourceRepoPath}");
        }

        private static void ExecuteCreateContentItem(Configuration config, ISlideDeckWriteRepository writeRepo)
        {
            // TODO: Validate inputs
            // TODO: Respect --SkipOutput switch
            var contentType = GetContentType(config.ContentPath);
            var content = System.IO.File.ReadAllBytes(config.ContentPath);
            var contentItemId = Guid.NewGuid();
            var contentItemTitle = config.Title;
            var contentItemFileName = System.IO.Path.GetFileName(config.ContentPath);

            writeRepo.WriteContentItem(contentItemId, contentType, contentItemFileName, contentItemTitle, content);
            Console.WriteLine($"ContentItem {contentItemId} ('{contentItemTitle}') written to {config.SourceRepoPath}");
        }

        private static void ExecuteCreateSlideDeck(Configuration config, ISlideDeckWriteRepository writeRepo)
        {
            // TODO: Validate inputs
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

        private static void ExecuteCloneSlideDeck(Configuration config, ISlideDeckReadRepository readRepo, ISlideDeckWriteRepository writeRepo)
        {
            // TODO: Validate inputs
            // TODO: Respect --SkipOutput switch
            var slideDeck = readRepo.GetSlideDeck(config.SlideDeckId);

            var newSlideDeck = slideDeck.Clone(Guid.NewGuid(), config.Title);
            writeRepo.SaveSlideDeck(newSlideDeck);
            Console.WriteLine($"Presentation {newSlideDeck.Id} ('{newSlideDeck.Title}') written to {config.SourceRepoPath}");
        }

        private static void ExecuteBuild(Configuration config, ISlideDeckReadRepository readRepo, IPresentationBuilder engine)
        {
            // Load a slide deck from a source repository
            // and build it into a RevealJS presentation

            config.SlideDeckId.ValidateNotNullOrEmpty("A valid Slide Deck must be specified");
            config.PresentationPath.ValidateNotNullOrEmpty("A valid Presentation Path must be specified");

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

        private static string GetContentType(string sourceFilePath)
        {
            string result = string.Empty;
            string cleanExtension = System.IO.Path.GetExtension(sourceFilePath).ToLower();
            switch (cleanExtension)
            {
                case ".md":
                    result = "text/markdown";
                    break;
                case ".png":
                    result = "image/png";
                    break;
                case ".jpg":
                case ".jfif":
                case ".jpeg":
                    result = "image/jpg";
                    break;
                case ".gif":
                    result = "image/gif";
                    break;
            }

            return result;
        }
    }
}
