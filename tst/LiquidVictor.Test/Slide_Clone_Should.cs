using System.Diagnostics.CodeAnalysis;

namespace LiquidVictor.Test;

[ExcludeFromCodeCoverage]
public class Slide_Clone_Should
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(true)]
    [InlineData(false)]
    public void ReturnANewSlideFromAMinimalSource(bool createNewId)
    {
        var source = new Entities.Slide();
        var target = source.Clone(createNewId);
        Assert.NotNull(target);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void PreserveTheShowFooterOverride(bool? showFooter)
    {
        var source = new Entities.Slide() { ShowFooter = showFooter };
        var target = source.Clone();
        Assert.Equal(showFooter, target.ShowFooter);
    }
}
