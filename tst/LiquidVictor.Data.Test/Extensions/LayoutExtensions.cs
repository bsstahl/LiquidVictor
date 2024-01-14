using LiquidVictor.Enumerations;

namespace LiquidVictor.Data.Test.Extensions;

public static class LayoutExtensions
{
    public static Layout GetRandom(this Layout _)
    {
        var values = Enum.GetValues<Layout>();
        var random = new Random();
        return values[random.Next(values.Length)];
    }
}
