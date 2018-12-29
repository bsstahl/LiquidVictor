using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiquidVictor.Data.Postgres
{
    public static class SlideCollectionExtensions
    {

        internal static ICollection<KeyValuePair<int, Entities.Slide>> AsEntities(this IEnumerable<SlideDeckSlide> slideDeckSlides)
        {
            var result = new List<KeyValuePair<int, Entities.Slide>>();
            foreach (var slideDeckSlide in slideDeckSlides)
                result.Add(new KeyValuePair<int, Entities.Slide>(slideDeckSlide.SortOrder, slideDeckSlide.Slide.AsEntity()));
            return result;
        }

    }
}
