using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;

namespace LiquidVictor.Data.Hardcoded
{
    public static class SlideExtensions
    {
        public static void Add(this List<KeyValuePair<int, Entities.Slide>> list, int sortOrder, Entities.Slide slide)
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Add(new KeyValuePair<int, Entities.Slide>(sortOrder, slide));
        }

        public static void Add(this List<Slide> list,
            int sortOrder, string slideTitle, Layout layout,
            string primaryContent, string primaryContentType)
        {
            ArgumentNullException.ThrowIfNull(list);
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
            list.Add(slide);
        }

        public static void Add(this List<Slide> list,
            int sortOrder, string slideTitle, Layout layout,
            string primaryContent, string primaryContentType,
            string primaryImageTitle, string primaryImageSource,
            string primaryImageContentType)
        {
            ArgumentNullException.ThrowIfNull(list);
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

            list.Add(slide);
        }

    }
}