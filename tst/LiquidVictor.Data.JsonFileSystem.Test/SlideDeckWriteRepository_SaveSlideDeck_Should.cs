using LiquidVictor.Builders;
using LiquidVictor.Data.Test.Extensions;
using LiquidVictor.Exceptions;
using TestHelperExtensions;

namespace LiquidVictor.Data.JsonFileSystem;

public class SlideDeckWriteRepository_SaveSlideDeck_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowDuplicateEntityIdException_WhenTwoSlidesInTheDeckShareAnId()
    {
        var slideId = Guid.NewGuid();
        var repoPath = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());

        var slideDeck = new SlideDeckBuilder()
            .Id(Guid.NewGuid())
            .Title(string.Empty.GetRandom())
            .SubTitle(string.Empty.GetRandom())
            .Presenter(string.Empty.GetRandom())
            .PrintLinkText(string.Empty.GetRandom())
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder().UseRandomValues().Id(slideId))
                .Add(new SlideBuilder().UseRandomValues())
                .Add(new SlideBuilder().UseRandomValues().Id(slideId)))
            .Build();

        var repo = new SlideDeckWriteRepository(repoPath);

        var ex = Assert.Throws<DuplicateEntityIdException>(() => repo.SaveSlideDeck(slideDeck));
        Assert.Equal("Slide", ex.EntityType);
        Assert.Contains(slideId, ex.DuplicateIds);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowDuplicateEntityIdException_WhenTwoContentItemsInTheDeckShareAnId()
    {
        var contentItemId = Guid.NewGuid();
        var repoPath = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());

        var slideDeck = new SlideDeckBuilder()
            .Id(Guid.NewGuid())
            .Title(string.Empty.GetRandom())
            .SubTitle(string.Empty.GetRandom())
            .Presenter(string.Empty.GetRandom())
            .PrintLinkText(string.Empty.GetRandom())
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder()
                    .UseRandomValues()
                    .ContentItems(new ContentItemBuilder().UseRandomValues().Id(contentItemId)))
                .Add(new SlideBuilder()
                    .UseRandomValues()
                    .ContentItems(new ContentItemBuilder().UseRandomValues().Id(contentItemId))))
            .Build();

        var repo = new SlideDeckWriteRepository(repoPath);

        var ex = Assert.Throws<DuplicateEntityIdException>(() => repo.SaveSlideDeck(slideDeck));
        Assert.Equal("ContentItem", ex.EntityType);
        Assert.Contains(contentItemId, ex.DuplicateIds);
    }
}
