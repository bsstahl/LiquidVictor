using System;
using System.Collections.Generic;
using LiquidVictor.Interfaces;

namespace LiquidVictor.Data.JsonFileSystem
{
    public class SlideDeckReadRepository : Interfaces.ISlideDeckReadRepository
    {
        readonly string _repositoryPath;

        public SlideDeckReadRepository(string repositoryPath)
        {
            _repositoryPath = repositoryPath;
        }

        public Entities.SlideDeck GetSlideDeck(Guid id)
        {
            var slideDeckPath = System.IO.Path.Combine(_repositoryPath, "SlideDecks");
            var existingFileName = slideDeckPath.FindFileWithId(id);

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
            var sourceFolderPath = System.IO.Path.GetDirectoryName(_repositoryPath);
            var slidePath = System.IO.Path.Combine(sourceFolderPath, $"Slides\\{id}.json");
            var slideJson = System.IO.File.ReadAllText(slidePath);
            var slide = Newtonsoft.Json.JsonConvert.DeserializeObject<Slide>(slideJson);

            int contentItemIndex = 0;
            var contentItems = new List<KeyValuePair<int, Entities.ContentItem>>();
            foreach (var contentItemId in slide.ContentItemIds)
            {
                var contentItemPair = this.GetContentItemPair(contentItemIndex, Guid.Parse(contentItemId));
                contentItems.Add(contentItemPair);
                contentItemIndex++;
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
            var sourceFolderPath = System.IO.Path.GetDirectoryName(_repositoryPath);
            var contentItemsPath = System.IO.Path.Combine(sourceFolderPath, "ContentItems");
            var contentItemPath = System.IO.Path.Combine(contentItemsPath, $"{id}.json");
            var contentItemJson = System.IO.File.ReadAllText(contentItemPath);
            var localContentItem = Newtonsoft.Json.JsonConvert.DeserializeObject<ContentItem>(contentItemJson);

            var contentItemEntity = new Entities.ContentItem()
            {
                Id = id,
                ContentType = localContentItem.ContentType,
                FileName = localContentItem.FileName,
                Title = localContentItem.Title,
                Content = ContentItem.DecodeContent(localContentItem.ContentType, localContentItem.EncodedContent)
            };

            return contentItemEntity;
        }

    }
}
