namespace LiquidVictor.Test;

public class Slide_Clone_Should
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ReturnANewSlideFromAMinimalSource(bool createNewId)
    {
        var source = new Entities.Slide();
        var target = source.Clone(createNewId);
        Assert.NotNull(target);
    }
}