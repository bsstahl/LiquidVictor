using System.Diagnostics.CodeAnalysis;

namespace LiquidVictor.Data.YamlFile.Test;

[ExcludeFromCodeCoverage]
public class SlideDeckReadRepository_GetSlideDeck_Should
{
    [Fact]
    [Trait("Category", "Integration")]
    public void ReturnTheSampleSlideDeckWithOneIndividualSlide()
    {
        var repoPath = Path.GetFullPath(@".\TestRepo");
        var slideDeckId = Guid.Parse("64b458e1-be36-4f42-86b0-63f7a9ca1503");

        var repo = new SlideDeckReadRepository(repoPath);
        var result = repo.GetSlideDeck(slideDeckId);

        var slide = result.Slides.Single().Value;
        var contentItem1 = slide.ContentItems.First().Value;
        var contentItem2 = slide.ContentItems.Last().Value;

        Assert.Equal(slideDeckId, result.Id);
        Assert.Equal("Test Deck 1", result.Title);
        Assert.Equal("A Test of Liquid Victor", result.SubTitle);
        Assert.Equal("Barry S. Stahl", result.Presenter);
        Assert.Equal("moon", result.ThemeName);
        Assert.Equal("Widescreen", result.AspectRatio.ToString());
        Assert.Equal("Slide", result.Transition.ToString());
        Assert.Equal("https://example.com/TestDeck1", result.SlideDeckUrl?.ToString());

        Assert.Equal("833f8eae-471f-4f6d-9493-eb18dd6d4f5e", slide.Id.ToString());
        Assert.Equal("Test Slide 1", slide.Title);
        Assert.Equal("PresentationDefault", slide.TransitionIn.ToString());
        Assert.Equal("PresentationDefault", slide.TransitionOut.ToString());
        Assert.Null(slide.BackgroundContent);
        Assert.Equal("My 1st Test Slide", slide.Notes);

        Assert.Equal("187957ad-8831-4498-89ce-fcd76ecc26aa", contentItem1.Id.ToString());
        Assert.Equal("image/jpg", contentItem1.ContentType);
        Assert.Equal("Manifest Asynchrony.jpeg", contentItem1.FileName);
        Assert.Equal("Manifest Asynchrony", contentItem1.Title);
        Assert.Equal(304769, contentItem1.Content.Length);

        Assert.Equal("4f1e0289-fb19-42a5-9cc1-9d343cc8ee12", contentItem2.Id.ToString());
        Assert.Equal("text/markdown", contentItem2.ContentType);
        Assert.Null(contentItem2.FileName);
        Assert.Empty(contentItem2.Title);
        Assert.Equal(103, contentItem2.Content.Length);
    }
}