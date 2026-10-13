using LiquidVictor.Builders;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using LiquidVictor.Output.RevealJs.Extensions;

namespace LiquidVictor.Output.RevealJs.Test;

public class SlideDeckExtensions_CreateIndexSlide_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowArgumentNullException_WhenSlideDeckIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => SlideDeckExtensions.CreateIndexSlide(null!));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnIndexSlideWithFullPageLayoutAndTitle()
    {
        var slideDeck = new SlideDeckBuilder()
            .Title("Test Deck")
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder()
                    .Title("Section 1")
                    .Layout(Enumerations.Layout.FullPage)
                    .IsSectionHeading(true)))
            .Build();

        var indexSlide = slideDeck.CreateIndexSlide();

        Assert.NotNull(indexSlide);
        Assert.Equal("Index", indexSlide.Title);
        Assert.Equal(Enumerations.Layout.FullPage, indexSlide.Layout);
        Assert.NotEqual(Guid.Empty, indexSlide.Id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void IncludeLinksOnlyForSectionHeadingSlidesUsingTheirTitles()
    {
        var section1Id = Guid.NewGuid();
        var normalSlideId = Guid.NewGuid();
        var section2Id = Guid.NewGuid();

        var slideDeck = new SlideDeckBuilder()
            .Title("Test Deck")
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder()
                    .Id(section1Id)
                    .Title("First Section")
                    .Layout(Enumerations.Layout.FullPage)
                    .IsSectionHeading(true))
                .Add(new SlideBuilder()
                    .Id(normalSlideId)
                    .Title("Regular Slide")
                    .Layout(Enumerations.Layout.FullPage)
                    .IsSectionHeading(false))
                .Add(new SlideBuilder()
                    .Id(section2Id)
                    .Title("Second Section")
                    .Layout(Enumerations.Layout.FullPage)
                    .IsSectionHeading(true)))
            .Build();

        var indexSlide = slideDeck.CreateIndexSlide();

        var contentItem = Assert.Single(indexSlide.ContentItems).Value;
        Assert.Equal("text/markdown", contentItem.ContentType);

        var markdown = contentItem.Content.AsString();
        Assert.Contains($"* [First Section](#{section1Id})", markdown);
        Assert.Contains($"* [Second Section](#{section2Id})", markdown);
        Assert.DoesNotContain("Regular Slide", markdown);
        Assert.DoesNotContain(normalSlideId.ToString(), markdown);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void UseSlideIdAsLinkText_WhenSectionHeadingTitleIsEmpty()
    {
        var sectionId = Guid.NewGuid();

        var slideDeck = new SlideDeckBuilder()
            .Title("Test Deck")
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder()
                    .Id(sectionId)
                    .Title(string.Empty)
                    .Layout(Enumerations.Layout.FullPage)
                    .IsSectionHeading(true)))
            .Build();

        var indexSlide = slideDeck.CreateIndexSlide();

        var contentItem = Assert.Single(indexSlide.ContentItems).Value;
        var markdown = contentItem.Content.AsString();
        Assert.Contains($"* [{sectionId}](#{sectionId})", markdown);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void OrderLinksBySlideDeckOrder()
    {
        var section1Id = Guid.NewGuid();
        var section2Id = Guid.NewGuid();

        var slideDeck = new SlideDeckBuilder()
            .Title("Test Deck")
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder()
                    .Id(section1Id)
                    .Title("Part A")
                    .Layout(Enumerations.Layout.FullPage)
                    .IsSectionHeading(true))
                .Add(new SlideBuilder()
                    .Id(section2Id)
                    .Title("Part B")
                    .Layout(Enumerations.Layout.FullPage)
                    .IsSectionHeading(true)))
            .Build();

        var indexSlide = slideDeck.CreateIndexSlide();
        var content = Assert.Single(indexSlide.ContentItems).Value.Content.AsString();

        var indexA = content.IndexOf("Part A", StringComparison.Ordinal);
        var indexB = content.IndexOf("Part B", StringComparison.Ordinal);

        Assert.True(indexA < indexB);
    }
}
