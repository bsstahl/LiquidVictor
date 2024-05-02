using LiquidVictor.Entities;
using System.Collections.Generic;

namespace LiquidVictor.Extensions;

internal static class SlideExtensions
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
}