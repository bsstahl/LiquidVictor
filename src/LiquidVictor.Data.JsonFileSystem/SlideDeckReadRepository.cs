using System;
using System.Collections.Generic;
using System.Linq;
using LiquidVictor.Exceptions;
using LiquidVictor.Extensions;
using LiquidVictor.Interfaces;

namespace LiquidVictor.Data.JsonFileSystem
{
    public class SlideDeckReadRepository : Interfaces.ISlideDeckReadRepository
    {
        readonly string _repositoryPath;
        readonly string _sourceFolderPath;
        readonly string _slideDeckPath;
        readonly string _slidesPath;
        readonly string _contentItemsPath;

        public SlideDeckReadRepository(string repositoryPath)
        {
            _repositoryPath = repositoryPath;
            _sourceFolderPath = System.IO.Path.GetDirectoryName(_repositoryPath);
            _slideDeckPath = System.IO.Path.Combine(_repositoryPath, "SlideDecks");
            _contentItemsPath = System.IO.Path.Combine(_repositoryPath, "ContentItems");
            _slidesPath = System.IO.Path.Combine(_repositoryPath, "Slides");
        }

        public IEnumerable<Guid> GetSlideDeckIds()
        {
            return _slideDeckPath.GetFileIds();
        }

        public IEnumerable<Guid> GetSlideIds()
        {
            return _slidesPath.GetFileIds();
        }

        public IEnumerable<Guid> GetContentItemIds()
        {
            return _contentItemsPath.GetFileIds();
        }

        public Entities.SlideDeck GetSlideDeck(Guid id)
        {
            var existingFileName = _slideDeckPath.FindFileWithId(id);
            if (string.IsNullOrEmpty(existingFileName))
                throw new SlideDeckNotFoundException(id, _slideDeckPath);

            var slideDeckJson = System.IO.File.ReadAllText(existingFileName);
            var slideDeck = Newtonsoft.Json.JsonConvert.DeserializeObject<SlideDeck>(slideDeckJson);

            var slides = new List<KeyValuePair<int, Entities.Slide>>();
            int slideIndex = 0;
            foreach (var slideId in slideDeck.SlideIds)
            {
                var slidePair = this.GetSlidePair(slideIndex, Guid.Parse(slideId));
                slides.Add(slidePair);
                slideIndex++;
            }

            Enumerations.AspectRatio aspectRatio = (Enumerations.AspectRatio)Enum.Parse(typeof(Enumerations.AspectRatio), slideDeck.AspectRatio);
            var slideDeckId = Guid.Parse(slideDeck.Id);
            var slideDeckTransition = slideDeck.GetTransition();
            var result = new Entities.SlideDeck(slideDeckId, slideDeck.Title, slideDeck.SubTitle, slideDeck.Presenter, slideDeck.ThemeName, slideDeck.SlideDeckUrl, slideDeck.PrintLinkText, slideDeckTransition, aspectRatio, slides);

            return result;
        }

        private KeyValuePair<int, Entities.ContentItem> GetContentItemPair(int contentItemIndex, Guid contentItemId)
        {
            return new KeyValuePair<int, Entities.ContentItem>(contentItemIndex, this.GetContentItem(contentItemId));
        }

        private KeyValuePair<int, Entities.Slide> GetSlidePair(int slideIndex, Guid slideId)
        {
            return new KeyValuePair<int, Entities.Slide>(slideIndex, this.GetSlide(slideId));
        }

        public Entities.Slide GetSlide(Guid id)
        {
            var slidePath = System.IO.Path.Combine(_slidesPath, $"{id}.json");
            var slideJson = System.IO.File.ReadAllText(slidePath);
            var slide = Newtonsoft.Json.JsonConvert.DeserializeObject<Slide>(slideJson);

            int contentItemIndex = 0;
            var contentItems = new List<KeyValuePair<int, Entities.ContentItem>>();
            foreach (var contentItemId in slide.ContentItemIds)
            {
                try
                {
                    var contentItemPair = this.GetContentItemPair(contentItemIndex, Guid.Parse(contentItemId));
                    contentItems.Add(contentItemPair);
                    contentItemIndex++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to load Slide {id} due to error: '{ex.Message}'.");
                    throw;
                }
            }

            Guid? backgroundContentItemId = null;
            if (Guid.TryParse(slide.BackgroundContent, out var parsedValue))
                backgroundContentItemId = parsedValue;

            var slideResult = new Entities.Slide()
            {
                Id = id,
                Layout = (Enumerations.Layout)Enum.Parse(typeof(Enumerations.Layout), slide.Layout),
                Notes = slide.Notes,
                Title = slide.Title,
                TransitionIn = (Enumerations.Transition)Enum.Parse(typeof(Enumerations.Transition), slide.TransitionIn),
                TransitionOut = (Enumerations.Transition)Enum.Parse(typeof(Enumerations.Transition), slide.TransitionOut),
                BackgroundContent = backgroundContentItemId.HasValue ? this.GetContentItem(backgroundContentItemId.Value) : null,
                NeverFullScreen = slide.NeverFullScreen,
                ContentItems = contentItems
            };

            return slideResult;
        }

        public Entities.ContentItem GetContentItem(Guid id)
        {
            var contentItemPath = System.IO.Path.Combine(_contentItemsPath, $"{id}.json");
            var contentItemJson = System.IO.File.ReadAllText(contentItemPath);
            var localContentItem = Newtonsoft.Json.JsonConvert.DeserializeObject<ContentItem>(contentItemJson);

            var contentItemEntity = new Entities.ContentItem()
            {
                Id = id,
                Alignment = localContentItem.Alignment,
                ContentType = localContentItem.ContentType,
                FileName = localContentItem.FileName,
                Title = localContentItem.Title,
                Content = localContentItem.EncodedContent.DecodeContent(localContentItem.ContentType)
            };

            return contentItemEntity;
        }

        public IEnumerable<Entities.SlideDeck> GetSlideDecks()
        {
            var result = new List<Entities.SlideDeck>();
            var slideDeckIds = this.GetSlideDeckIds();
            foreach (var slideDeckId in slideDeckIds)
            {
                result.Add(this.GetSlideDeck(slideDeckId));
            }

            return result;
        }

        public IEnumerable<Entities.Slide> GetSlides()
        {
            var result = new List<Entities.Slide>();
            var slideIds = this.GetSlideIds();
            foreach (var slideId in slideIds)
            {
                result.Add(this.GetSlide(slideId));
            }
            return result;
        }

        public IEnumerable<Entities.ContentItem> GetContentItems()
        {
            var result = new List<Entities.ContentItem>();
            var contentItemIds = this.GetContentItemIds();
            foreach (var contentItemId in contentItemIds)
            {
                result.Add(this.GetContentItem(contentItemId));
            }
            return result;
        }

        internal static (IEnumerable<Guid> SlideDeckIds, IEnumerable<Guid> SlideIds, IEnumerable<Guid> ContentItemIds) FindDuplicateIds(IEnumerable<Entities.SlideDeck> slideDecks)
        {
            var slides = slideDecks.SelectMany(d => d.Slides).Select(s => s.Value);
            var contentItems = slides.SelectMany(s => s.ContentItems).Select(c => c.Value);

            var deckIds = slideDecks
                .GroupBy(d => d.Id)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .Where(c => c.Count > 1)
                .ToList();

            var slideIds = slides
                .GroupBy(s => s.Id)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .Where(c => c.Count > 1)
                .ToList();

            var contentItemIds = contentItems
                .GroupBy(c => c.Id)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .Where(c => c.Count > 1)
                .ToList();

            return (Array.Empty<Guid>(), Array.Empty<Guid>(), Array.Empty<Guid>());
        }
    }
}
