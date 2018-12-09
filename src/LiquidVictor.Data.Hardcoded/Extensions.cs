using LiquidVictor.Enumerations;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Data.Hardcoded
{
    public static class Extensions
    {
        public static void Add(this List<KeyValuePair<int, Entities.Slide>> list, int index, Entities.Slide slide)
        {
            list.Add(new KeyValuePair<int, Entities.Slide>(index, slide));
        }

        public static void Add(this List<KeyValuePair<int, Entities.Slide>> list,
            int sortOrder, string slideTitle, Layout layout,
            string primaryContent, string primaryContentType)
        {
            var slide = new Entities.Slide()
            {
                Title = slideTitle,
                Layout = layout,
                ContentItems = new List<KeyValuePair<int, Entities.ContentItem>>()
                    {
                        new KeyValuePair<int, Entities.ContentItem>(
                            sortOrder,
                            new Entities.ContentItem()
                                {
                                    ContentType = primaryContentType,
                                    Content = primaryContent.AsByteArray()
                                })
                    }
            };

            list.Add(sortOrder, slide);
        }

        public static void Add(this List<KeyValuePair<int, Entities.Slide>> list,
            int sortOrder, string slideTitle, Layout layout,
            string primaryContent, string primaryContentType,
            string primaryImageTitle, string primaryImageSource, 
            string primaryImageContentType)
        {
            var slide = new Entities.Slide()
            {
                Title = slideTitle,
                Layout = layout,
                ContentItems = new List<KeyValuePair<int, Entities.ContentItem>>()
                    {
                        new KeyValuePair<int, Entities.ContentItem>(
                            sortOrder,
                            new Entities.ContentItem()
                                {
                                    ContentType = primaryContentType,
                                    Content = primaryContent.AsByteArray()
                                }),
                        new KeyValuePair<int, Entities.ContentItem>(
                            sortOrder,
                            new Entities.ContentItem()
                                {
                                    ContentType = primaryImageContentType,
                                    Content = primaryImageSource.GetImageContent(),
                                    Title = primaryImageTitle
                                })
                    }
            };
        }

        public static byte[] GetImageContent(this string imageSource)
        {
            return System.IO.File.ReadAllBytes(imageSource);
        }
    }
}