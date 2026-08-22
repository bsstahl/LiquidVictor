using LiquidVictor.Enumerations;

namespace LiquidVictor.Data.Test.Extensions;

[ExcludeFromCodeCoverage]
public static class TransitionExtensions
{
    public static Transition GetRandom(this Transition _)
    {
        var values = Enum.GetValues<Transition>();
        var random = new Random();
        return values[random.Next(values.Length)];
    }
}
