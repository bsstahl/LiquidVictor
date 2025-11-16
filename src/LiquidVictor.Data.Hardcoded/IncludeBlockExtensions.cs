using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using System;
using System.Collections.Generic;

namespace LiquidVictor.Data.Hardcoded
{
    public static class IncludeBlockExtensions
    {
        public static void Add(this List<IncludeBlock> list,
            int sortOrder, string slideTitle, Layout layout,
            string primaryContent, string primaryContentType)
        {
            var contentItems = new List<KeyValuePair<int, Entities.ContentItem>>()
            {
                new KeyValuePair<int, Entities.ContentItem>(
                    sortOrder,
                    new Entities.ContentItem()
                        {
                            ContentType = primaryContentType,
                            Content = primaryContent.AsByteArray()
                        })
            };

            var slide = new Entities.Slide(Guid.NewGuid(), slideTitle, layout, Transition.PresentationDefault, Transition.PresentationDefault, string.Empty, null, false, contentItems);
            list.Add(new IncludeBlock(slide));
        }

        public static void Add(this List<IncludeBlock> list,
            int sortOrder, string slideTitle, Layout layout,
            string primaryContent, string primaryContentType,
            string primaryImageTitle, string primaryImageSource,
            string primaryImageContentType)
        {
            var slide = new Entities.Slide()
            {
                Title = slideTitle,
                Layout = layout
            };

            slide.ContentItems.Add(new KeyValuePair<int, Entities.ContentItem>(
                        sortOrder,
                        new Entities.ContentItem()
                        {
                            ContentType = primaryContentType,
                            Content = primaryContent.AsByteArray()
                        }));
            
            slide.ContentItems.Add(
                    new KeyValuePair<int, Entities.ContentItem>(
                        sortOrder,
                        new Entities.ContentItem()
                        {
                            ContentType = primaryImageContentType,
                            Content = primaryImageSource.GetImageContent(),
                            Title = primaryImageTitle
                        }));

            list.Add(new IncludeBlock(slide));
        }

    }
}