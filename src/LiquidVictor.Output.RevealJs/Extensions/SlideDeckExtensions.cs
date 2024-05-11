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
                Layout = Enumerations.Layout.Title
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

            string url = slideDeck.SlideDeckUrl.ToString() ?? " ";
            titleSlide.ContentItems.Add(
                new KeyValuePair<int, ContentItem>(3,
                new ContentItem()
                {
                    Content = url.AsByteArray(),
                    ContentType = "text/plain",
                    Id = Guid.NewGuid()
                }));

            string linkText = slideDeck.PrintLinkText ?? " ";
            titleSlide.ContentItems.Add(
                new KeyValuePair<int, ContentItem>(3,
                new ContentItem()
                {
                    Content = linkText.AsByteArray(),
                    ContentType = "text/plain",
                    Id = Guid.NewGuid()
                }));

            return titleSlide;
        }

        public static (int, int) GetPresentationSize(this SlideDeck deck)
        {
            int width, height;
            switch (deck.AspectRatio)
            {
                case Enumerations.AspectRatio.Widescreen: // 16:9
                    width = 1920;
                    height = 1080;
                    break;
                case Enumerations.AspectRatio.Standard: // 4:3
                    width = 1024;
                    height = 768;
                    break;
                default:
                    throw new NotSupportedException($"Invalid Aspect Ratio {deck.AspectRatio}");
            }

            return (width, height);
        }
    }
}
