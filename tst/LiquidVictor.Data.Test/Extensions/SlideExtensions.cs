namespace LiquidVictor.Data.Test.Extensions;

public static class SlideExtensions
{
    public static IEnumerable<KeyValuePair<int, Entities.Slide>> AsOrderdPairs(this IEnumerable<Entities.Slide> items)
    => items.Select((i, j) => new KeyValuePair<int, Entities.Slide>(j, i));
}
