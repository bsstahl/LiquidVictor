using LiquidVictor.Entities;
using LiquidVictor.Extensions;
using System;
using System.Text;

namespace LiquidVictor.Strategy.TableOfContents
{
    public class Engine : Interfaces.ITableOfContentsStrategy
    {
        public Slide GetContentsSlide(SlideDeck slideDeck)
        {
            var md = this.GetMarkdown(slideDeck);

            var contentItem = new ContentItem()
            {
                Id = Guid.NewGuid(),
                ContentType = "text/markdown",
                FileName = null,
                Title = "Table of Contents",
                Content = md.AsByteArray()
            };

            var contentItemPair = new KeyValuePair<int, ContentItem>(1, contentItem);

            return new Slide()
            {
                Id = Guid.NewGuid(),
                Title = "Table of Contents",
                NeverFullScreen = false,
                Layout = Enumerations.Layout.FullPage,
                TransitionIn = Enumerations.Transition.PresentationDefault,
                TransitionOut = Enumerations.Transition.PresentationDefault,
                BackgroundContent = null,
                Notes = String.Empty,
                ContentItems = new List<KeyValuePair<int, ContentItem>>() { contentItemPair }
            };
        }

        public string GetMarkdown(SlideDeck slideDeck)
            => GetMarkdown(slideDeck, false);

        public string GetMarkdown(SlideDeck slideDeck, bool prettyPrint)
        {
            var toc = this.GetTableOfContents(slideDeck);

            var sb = new StringBuilder();
            foreach (var entry in toc.Entries.OrderBy(s => s.SlideIndexInDeck))
            {
                var title = entry.Title.NullIfEmpty() ?? entry.SlideId.ToString();
                sb.AppendLine($"* [{title}](#{entry.SlideId})");
            }

            return prettyPrint
                ? sb.ToString()
                : sb.ToString().Replace("\r\n", "\\r\\n");
        }

        public Entities.TableOfContents GetTableOfContents(Entities.SlideDeck slideDeck)
        {
            var entries = new List<Entities.TableOfContentsEntry>();
            foreach (var slide in slideDeck.Slides.OrderBy(s => s.Key))
            {
                entries.Add(new Entities.TableOfContentsEntry()
                {
                    SlideId = slide.Value.Id,
                    Title = slide.Value.Title,
                    SlideIndexInDeck = slide.Key,
                    ContentItemTitles = slide.Value.ContentItems.Select(ci => ci.Value.Title)
                });
            }

            return new Entities.TableOfContents()
            {
                SlideDeckId = slideDeck.Id,
                SlideDeckTitle = slideDeck.Title,
                SlideDeckSubTitle = slideDeck.SubTitle,
                SlideDeckPresenter = slideDeck.Presenter,
                SlideDeckUrl = slideDeck.SlideDeckUrl,
                Entries = entries
            };
        }
    }
}