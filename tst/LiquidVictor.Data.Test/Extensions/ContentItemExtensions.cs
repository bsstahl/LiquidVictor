namespace LiquidVictor.Data.Test.Extensions;

public static class ContentItemExtensions
{
    public static IEnumerable<KeyValuePair<int, Entities.ContentItem>> AsOrderdPairs(this IEnumerable<Entities.ContentItem> items)
        => items.Select((i, j) => new KeyValuePair<int, Entities.ContentItem>(j, i));
}
