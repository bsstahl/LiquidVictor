using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Entities;

namespace LiquidVictor.Data.JsonFileSystem
{
    public class SlideDeckWriteRepository : Interfaces.ISlideDeckWriteRepository
    {
        string _sourceFolderPath;

        public SlideDeckWriteRepository(string sourceFolderPath)
        {
            _sourceFolderPath = System.IO.Path.GetFullPath(sourceFolderPath);
        }

        public void SaveSlideDeck(Entities.SlideDeck slideDeck)
        {
            var sd = new JsonFileSystem.SlideDeck()
            {
                Id = slideDeck.Id.ToString(),
                AspectRatio = slideDeck.AspectRatio.ToString(),
                Presenter = slideDeck.Presenter,
                SubTitle = slideDeck.SubTitle,
                ThemeName = slideDeck.ThemeName,
                Title = slideDeck.Title,
                SlideIds = slideDeck.Slides.OrderBy(s => s.Key).Select(s => s.Value.Id.ToString()).ToArray()
            };

            var slideDeckFileName = GetSlideDeckFileName(slideDeck.Id, slideDeck.Title);
            string slideDeckPath = System.IO.Path.Combine(_sourceFolderPath, $"{slideDeckFileName}.json");

            // Create folder structure if necessary
            if (!System.IO.Directory.Exists(_sourceFolderPath))
                System.IO.Directory.CreateDirectory(_sourceFolderPath);

            // Write SlideDeck file
            System.IO.File.WriteAllText(slideDeckPath, Newtonsoft.Json.JsonConvert.SerializeObject(sd));

            // Write Slides
            // TODO: Deduplicate (in case a slide is used more than once in a presentation)
            foreach (var s in slideDeck.Slides)
                this.SaveSlide(s.Value);
        }

        public void SaveSlide(Entities.Slide s)
        {
            string slidesPath = System.IO.Path.Combine(_sourceFolderPath, "Slides");
            if (!System.IO.Directory.Exists(slidesPath))
                System.IO.Directory.CreateDirectory(slidesPath);

            var slide = new JsonFileSystem.Slide()
            {
                Layout = s.Layout.ToString(),
                Notes = s.Notes,
                Title = s.Title,
                TransitionIn = s.TransitionIn.ToString(),
                TransitionOut = s.TransitionOut.ToString(),
                ContentItemIds = s.ContentItems.OrderBy(ci => ci.Key).Select(ci => ci.Value.Id.ToString()).ToArray()
            };

            // Write slide file
            string slidePath = System.IO.Path.Combine(slidesPath, $"{s.Id}.json");
            slide.SerializeTo(slidePath);

            // Write ContentItems
            foreach (var ci in s.ContentItems)
                SaveContentItem(ci.Value);
        }

        public void SaveContentItem(Entities.ContentItem contentItem)
        {
            var ci = new JsonFileSystem.ContentItem()
            {
                ContentType = contentItem.ContentType,
                EncodedContent = ContentItem.EncodeContent(contentItem.ContentType, contentItem.Content),
                FileName = contentItem.FileName,
                Title = contentItem.Title
            };

            string contentItemsPath = System.IO.Path.Combine(_sourceFolderPath, "ContentItems");
            if (!System.IO.Directory.Exists(contentItemsPath))
                System.IO.Directory.CreateDirectory(contentItemsPath);

            // Write ContentItem file
            string contentItemPath = System.IO.Path.Combine(contentItemsPath, $"{contentItem.Id}.json");
            ci.SerializeTo(contentItemPath);
        }

        private string GetSlideDeckFileName(Guid slideDeckId, string slideDeckTitle)
        {
            // If the slide deck already exists (per the id), use that filename
            // otherwise, use a filename generated from the title of the presentation
            // was originally hardcoded to "SlideDeck.json"
            string result = _sourceFolderPath.FindFileWithId(slideDeckId);
            if (string.IsNullOrWhiteSpace(result))
            {
                result = slideDeckTitle.Replace(" ", string.Empty); // TODO: Remove special chars
                var filePath = System.IO.Path.Combine(_sourceFolderPath, $"{result}.json");
                if (System.IO.File.Exists(filePath))
                    throw new InvalidOperationException($"SlideDeck already exists at '{filePath}'");
            }
            return result;
        }

    }
}
