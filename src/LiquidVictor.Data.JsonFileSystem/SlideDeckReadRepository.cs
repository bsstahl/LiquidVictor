using System;
using System.Collections.Generic;
using LiquidVictor.Interfaces;

namespace LiquidVictor.Data.JsonFileSystem
{
    public class SlideDeckReadRepository : Interfaces.ISlideDeckReadRepository
    {
        readonly string _slideDeckPath;

        public SlideDeckReadRepository(string slideDeckPath)
        {
            _slideDeckPath = slideDeckPath;
        }

        public Entities.SlideDeck GetSlideDeck(Guid id)
        {
            var slideDeckJson = System.IO.File.ReadAllText(_slideDeckPath);
            var slideDeck = Newtonsoft.Json.JsonConvert.DeserializeObject<SlideDeck>(slideDeckJson);

            var slides = new List<KeyValuePair<int, Entities.Slide>>();
            int slideIndex = 0;
            foreach (var slideId in slideDeck.SlideIds)
            {
                var slidePair = GetSlide(slideIndex, slideId);
                slides.Add(slidePair);
                slideIndex++;
            }

            Enumerations.AspectRatio aspectRatio = (Enumerations.AspectRatio)Enum.Parse(typeof(Enumerations.AspectRatio), slideDeck.AspectRatio);
            var slideDeckId = Guid.Parse(slideDeck.Id);
            var slideDeckTransition = slideDeck.GetTransition();
            var result = new Entities.SlideDeck(slideDeckId, slideDeck.Title, slideDeck.SubTitle, slideDeck.Presenter, slideDeck.ThemeName, slideDeckTransition, aspectRatio, slides);

            return result;
        }

        private KeyValuePair<int, Entities.Slide> GetSlide(int slideIndex, string slideId)
        {
            var sourceFolderPath = System.IO.Path.GetDirectoryName(_slideDeckPath);
            var slidePath = System.IO.Path.Combine(sourceFolderPath, $"..\\Slides\\{ slideId}.json");
            var slideJson = System.IO.File.ReadAllText(slidePath);
            var slide = Newtonsoft.Json.JsonConvert.DeserializeObject<Slide>(slideJson);

            int contentItemIndex = 0;
            var contentItems = new List<KeyValuePair<int, Entities.ContentItem>>();
            foreach (var contentItemId in slide.ContentItemIds)
            {
                var contentItemPair = GetContentItem(contentItemIndex, contentItemId);
                contentItems.Add(contentItemPair);
                contentItemIndex++;
            }

            var slidePair = new KeyValuePair<int, Entities.Slide>(slideIndex,
                new Entities.Slide()
                {
                    Id = Guid.Parse(slideId),
                    Layout = (Enumerations.Layout)Enum.Parse(typeof(Enumerations.Layout), slide.Layout),
                    Notes = slide.Notes,
                    Title = slide.Title,
                    TransitionIn = (Enumerations.Transition)Enum.Parse(typeof(Enumerations.Transition), slide.TransitionIn),
                    TransitionOut = (Enumerations.Transition)Enum.Parse(typeof(Enumerations.Transition), slide.TransitionOut),
                    ContentItems = contentItems
                });

            return slidePair;
        }

        private KeyValuePair<int, Entities.ContentItem> GetContentItem(int contentItemIndex, string contentItemId)
        {
            var sourceFolderPath = System.IO.Path.GetDirectoryName(_slideDeckPath);
            var contentItemsPath = System.IO.Path.Combine(sourceFolderPath, @"..\ContentItems");
            var contentItemPath = System.IO.Path.Combine(contentItemsPath, $"{contentItemId}.json");
            var contentItemJson = System.IO.File.ReadAllText(contentItemPath);
            var contentItem = Newtonsoft.Json.JsonConvert.DeserializeObject<ContentItem>(contentItemJson);

            var contentItemPair = new KeyValuePair<int, Entities.ContentItem>(contentItemIndex,
                    new Entities.ContentItem()
                    {
                        Id = Guid.Parse(contentItemId),
                        ContentType = contentItem.ContentType,
                        FileName = contentItem.FileName,
                        Title = contentItem.Title,
                        Content = ContentItem.DecodeContent(contentItem.ContentType, contentItem.EncodedContent)
                    });
            return contentItemPair;
        }
    }
}
