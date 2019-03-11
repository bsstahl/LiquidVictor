using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class SlideDeckExtensions
    {
        public static Slide CreateTitleSlide(this SlideDeck slideDeck)
        {
            var titleSlide = new Slide()
            {
                Id = Guid.NewGuid(),
                Title = slideDeck.Title,
                Layout = Enumerations.Layout.Title,
                ContentItems = new List<KeyValuePair<int, ContentItem>>()
            };

            titleSlide.ContentItems.Add(
                new KeyValuePair<int, ContentItem>(1,
                new ContentItem()
                {
                    Content = slideDeck.SubTitle.AsByteArray(),
                    ContentType = "text/plain",
                    Id = Guid.NewGuid()
                }));

            titleSlide.ContentItems.Add(
                new KeyValuePair<int, ContentItem>(2,
                new ContentItem()
                {
                    Content = slideDeck.Presenter.AsByteArray(),
                    ContentType = "text/plain",
                    Id = Guid.NewGuid()
                }));

            if (!string.IsNullOrWhiteSpace(slideDeck.SlideDeckUrl))
            {
                titleSlide.ContentItems.Add(
                    new KeyValuePair<int, ContentItem>(3,
                    new ContentItem()
                    {
                        Content = slideDeck.SlideDeckUrl.AsByteArray(),
                        ContentType = "text/plain",
                        Id = Guid.NewGuid()
                    }));
            }

            return titleSlide;
        }

        public static (int, int) GetPresentationSize(this SlideDeck deck)
        {
            int width, height;
            switch (deck.AspectRatio)
            {
                case Enumerations.AspectRatio.Widescreen:
                    width = 1920;
                    height = 1080;
                    break;
                case Enumerations.AspectRatio.Standard:
                    width = 1024;
                    height = 768;
                    break;
                default:
                    throw new NotSupportedException($"Invalid Aspect Ratio {deck.AspectRatio.ToString()}");
            }

            return (width, height);
        }
    }
}
