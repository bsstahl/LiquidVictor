using LiquidVictor.Interfaces;
using System;
using System.Collections.Generic;

namespace LV
{
    internal static class ConfigurationExtensions
    {
        internal static void ExecuteValidateSourceRepo(this Configuration config, ISlideDeckReadRepository readRepo)
        {
            // Verifies that all SlideDecks have unique IDs
            var slideDeckIds = readRepo.GetSlideDeckIds();
            var validSlideDecks = new Dictionary<Guid, string>();
            foreach (var id in slideDeckIds)
            {
                // TODO: Add additional validations
                var slideDeck = readRepo.GetSlideDeck(id);
                validSlideDecks.Add(id, slideDeck.Title);
            }

            Console.WriteLine("Valid Slide Decks:");
            foreach (var slideDeck in validSlideDecks)
            {
                Console.WriteLine($"{slideDeck.Key} - {slideDeck.Value}");
            }
        }

        internal static void ExecuteCreateSlide(this Configuration config, ISlideDeckWriteRepository writeRepo)
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
                BackgroundContent = null,
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

        internal static void ExecuteCreateContentItem(this Configuration config, ISlideDeckWriteRepository writeRepo)
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

        internal static void ExecuteCreateSlideDeck(this Configuration config, ISlideDeckWriteRepository writeRepo)
        {
            // TODO: Validate inputs
            // TODO: Respect --SkipOutput switch
            var slideDeck = new LiquidVictor.Entities
                .SlideDeck(Guid.NewGuid(), config.Title, string.Empty, string.Empty, string.Empty, new List<KeyValuePair<int, LiquidVictor.Entities.Slide>>());
            writeRepo.SaveSlideDeck(slideDeck);
            Console.WriteLine($"Slide Deck {slideDeck.Id} ('{slideDeck.Title}') written to {config.SourceRepoPath}");
        }

        internal static void ExecuteCloneSlide(this Configuration config, ISlideDeckReadRepository readRepo, ISlideDeckWriteRepository writeRepo)
        {
            // TODO: Validate inputs
            // TODO: Respect --SkipOutput switch
            var slide = readRepo.GetSlide(config.SlideId);

            var newSlide = slide.Clone();
            newSlide.Id = Guid.NewGuid();

            writeRepo.SaveSlide(newSlide);
            Console.WriteLine($"Slide {newSlide.Id} ('{newSlide.Title}') written to {config.SourceRepoPath}");
        }

        internal static void ExecuteCloneSlideDeck(this Configuration config, ISlideDeckReadRepository readRepo, ISlideDeckWriteRepository writeRepo)
        {
            // TODO: Validate inputs
            // TODO: Respect --SkipOutput switch
            var slideDeck = readRepo.GetSlideDeck(config.SlideDeckId);

            var newSlideDeck = slideDeck.Clone(Guid.NewGuid(), config.Title);
            writeRepo.SaveSlideDeck(newSlideDeck);
            Console.WriteLine($"Presentation {newSlideDeck.Id} ('{newSlideDeck.Title}') written to {config.SourceRepoPath}");
        }

        internal static void ExecuteBuild(this Configuration config, ISlideDeckReadRepository readRepo, IPresentationBuilder engine)
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
