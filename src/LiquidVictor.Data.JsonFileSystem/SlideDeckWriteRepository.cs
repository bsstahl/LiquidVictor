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
            _sourceFolderPath = sourceFolderPath;
        }

        public void SaveSlideDeck(Entities.SlideDeck slideDeck)
        {
            string slideDeckPath = System.IO.Path.Combine(_sourceFolderPath, "SlideDeck.json");
            string slidesPath = System.IO.Path.Combine(_sourceFolderPath, "Slides");
            string contentItemsPath = System.IO.Path.Combine(_sourceFolderPath, "ContentItems");

            var sd = new SlideDeck()
            {
                Id = slideDeck.Id.ToString(),
                AspectRatio = slideDeck.AspectRatio.ToString(),
                Presenter = slideDeck.Presenter,
                SubTitle = slideDeck.SubTitle,
                ThemeName = slideDeck.ThemeName,
                Title = slideDeck.Title,
                SlideIds = slideDeck.Slides.OrderBy(s => s.Key).Select(s => s.Value.Id.ToString()).ToArray()
            };

            // Create folder structure if necessary
            if (!System.IO.Directory.Exists(_sourceFolderPath))
                System.IO.Directory.CreateDirectory(_sourceFolderPath);

            if (!System.IO.Directory.Exists(slidesPath))
                System.IO.Directory.CreateDirectory(slidesPath);

            if (!System.IO.Directory.Exists(contentItemsPath))
                System.IO.Directory.CreateDirectory(contentItemsPath);


            // Write SlideDeck file
            System.IO.File.WriteAllText(slideDeckPath, Newtonsoft.Json.JsonConvert.SerializeObject(sd));

            var contentItems = new List<ContentItem>();
            foreach (var s in slideDeck.Slides)
            {
                var slide = new Slide()
                {
                    Layout = s.Value.Layout.ToString(),
                    Notes = s.Value.Notes,
                    Title = s.Value.Title,
                    TransitionIn = s.Value.TransitionIn.ToString(),
                    TransitionOut = s.Value.TransitionOut.ToString(),
                    ContentItemIds = s.Value.ContentItems.OrderBy(ci => ci.Key).Select(ci => ci.Value.Id.ToString()).ToArray()
                };

                // Write slide file
                string slidePath = System.IO.Path.Combine(slidesPath, $"{s.Value.Id}.json");
                System.IO.File.WriteAllText(slidePath, Newtonsoft.Json.JsonConvert.SerializeObject(slide));

                foreach (var ci in s.Value.ContentItems)
                {
                    var contentItem = new ContentItem()
                    {
                        ContentType = ci.Value.ContentType,
                        FileName = ci.Value.FileName,
                        Title = ci.Value.Title,
                        EncodedContent = ContentItem.EncodeContent(ci.Value.ContentType, ci.Value.Content)
                    };

                    // Write ContentItem file
                    // TODO: Deduplicate (in case a content item is used more than once in a presentation)
                    string contentItemPath = System.IO.Path.Combine(contentItemsPath, $"{ci.Value.Id}.json");
                    System.IO.File.WriteAllText(contentItemPath, Newtonsoft.Json.JsonConvert.SerializeObject(contentItem));
                }

            }

        }
    }
}
