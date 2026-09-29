namespace LiquidVictor.Input.Powerpoint.Test;

public class SlideDeckImporter_GetSourceTag_Should
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("Deck.pptx", "pptx:Deck.pptx")]
    [InlineData("  Deck.pptx  ", "pptx:Deck.pptx")]
    [InlineData("Deck, Part 1.pptx", "pptx:Deck_ Part 1.pptx")]
    public void BuildATagFromTheFileName(string sourceName, string expected)
    {
        Assert.Equal(expected, SlideDeckImporter.GetSourceTag(sourceName));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void UseOnlyTheFileNameOfAPath()
    {
        var sourceName = Path.Combine(Path.GetTempPath(), "Folder", "Deck.pptx");
        Assert.Equal("pptx:Deck.pptx", SlideDeckImporter.GetSourceTag(sourceName));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NeverContainACommaSinceTagsArePersistedAsACommaDelimitedList()
    {
        Assert.DoesNotContain(",", SlideDeckImporter.GetSourceTag("a,b,c.pptx"), StringComparison.Ordinal);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData("   ")]
    public void ThrowWhenTheSourceNameIsBlank(string sourceName)
    {
        Assert.Throws<ArgumentException>(() => SlideDeckImporter.GetSourceTag(sourceName));
    }
}
