using System.Diagnostics.CodeAnalysis;

namespace LiquidVictor.Test;

[ExcludeFromCodeCoverage]
public class ContentItem_Clone_Should
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(true)]
    [InlineData(false)]
    public void ReturnANewContentItemFromAMinimalSource(bool createNewId)
    {
        var source = new Entities.ContentItem();
        source.Tags.Add("character");
        var target = source.Clone(createNewId);
        Assert.NotNull(target);
        Assert.Equal(["character"], target.Tags);
    }
}
