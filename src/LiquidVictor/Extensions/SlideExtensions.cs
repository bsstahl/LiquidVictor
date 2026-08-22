using LiquidVictor.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Extensions;

public static class SlideExtensions
{
    internal static ICollection<KeyValuePair<int, Slide>> Clone(this IEnumerable<KeyValuePair<int, Slide>> slidePairs, bool createNewId = false)
    {
        var result = new List<KeyValuePair<int, Slide>>();
        foreach (var pair in slidePairs)
        {
            result.Add(new KeyValuePair<int, Slide>(pair.Key, pair.Value.Clone(createNewId)));
        }
        return result;
    }

    public static IOrderedEnumerable<IncludeBlock> AsIncludeBlocks(this IEnumerable<KeyValuePair<int, Slide>> slides)
    {
        return slides
            .Select(pair => new IncludeBlock(pair.Value))
            .OrderBy(i => 0);
    }

    public static IncludeBlock AsIncludeBlock(this Slide slide) => new IncludeBlock(slide);
}