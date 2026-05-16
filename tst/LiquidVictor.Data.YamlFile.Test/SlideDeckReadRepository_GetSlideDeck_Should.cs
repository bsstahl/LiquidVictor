using LiquidVictor.Enumerations;
using System.Text;

namespace LiquidVictor.Data.YamlFile.Test;

public class SlideDeckReadRepository_GetSlideDeck_Should
{
    [Fact]
    [Trait("Category", "Integration")]
    public void ReturnTheSampleSlideDeck()
    {
        var repoPath = Path.Combine(AppContext.BaseDirectory, "TestRepo");
        var slideDeckId = Guid.Parse("64b458e1-be36-4f42-86b0-63f7a9ca1503");

        var repo = new SlideDeckReadRepository(repoPath);
        var result = repo.GetSlideDeck(slideDeckId);

        Assert.Equal(slideDeckId, result.Id);
        Assert.Equal("Test Deck 1", result.Title);
        Assert.Equal("A Test of Liquid Victor", result.SubTitle);
        Assert.Equal("Barry S. Stahl - Mastodon:@bsstahl@cognitiveinheritance.com - Blog:http://www.cognitiveinheritance.com", result.Presenter);
        Assert.Equal("moon", result.ThemeName);
        Assert.Equal(new Uri("https://example.com/TestDeck1"), result.SlideDeckUrl);
        Assert.Equal(AspectRatio.Widescreen, result.AspectRatio);
        Assert.Equal(Transition.Slide, result.Transition);

        var slide = Assert.Single(result.Slides).Value;
        Assert.Equal(Guid.Parse("833f8eae-471f-4f6d-9493-eb18dd6d4f5e"), slide.Id);
        Assert.Equal("Test Slide 1", slide.Title);
        Assert.Equal(Layout.ImageLeft, slide.Layout);
        Assert.Equal(Transition.PresentationDefault, slide.TransitionIn);
        Assert.Equal(Transition.PresentationDefault, slide.TransitionOut);
        Assert.Equal("My 1st Test Slide", slide.Notes);
        Assert.Null(slide.BackgroundContent);
        Assert.False(slide.NeverFullScreen);

        var contentItems = slide.ContentItems.Select(ci => ci.Value).ToDictionary(ci => ci.Id);
        Assert.Equal(2, contentItems.Count);

        Assert.True(contentItems.TryGetValue(Guid.Parse("4f1e0289-fb19-42a5-9cc1-9d343cc8ee12"), out var markdown));
        Assert.Equal("text/markdown", markdown.ContentType);
        Assert.True(string.IsNullOrEmpty(markdown.FileName));
        Assert.True(string.IsNullOrEmpty(markdown.Title));
        var markdownContent = Encoding.UTF8.GetString(markdown.Content).Replace("\r\n", Environment.NewLine);
        Assert.Equal(
            $"* This is some test content{Environment.NewLine}* It should be shown as bullet-points{Environment.NewLine}* Remember to Manifest Asynchrony{Environment.NewLine}",
            markdownContent);

        Assert.True(contentItems.TryGetValue(Guid.Parse("187957ad-8831-4498-89ce-fcd76ecc26aa"), out var image));
        Assert.Equal("image/jpg", image.ContentType);
        Assert.Equal("Manifest Asynchrony.jpeg", image.FileName);
        Assert.Equal("Manifest Asynchrony", image.Title);
        Assert.NotEmpty(image.Content);
    }
}
