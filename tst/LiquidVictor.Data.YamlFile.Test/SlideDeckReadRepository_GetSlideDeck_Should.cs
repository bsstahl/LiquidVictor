using LiquidVictor.Enumerations;
using System.Text;

namespace LiquidVictor.Data.YamlFile.Test;

public class SlideDeckReadRepository_GetSlideDeck_Should
{
    [Fact]
    [Trait("Category", "Integration")]
    public void ReturnTheSampleSlideDeckWithOneIndividualSlide()
    {
        var repoPath = Path.Combine(AppContext.BaseDirectory, "TestRepo");
        var slideDeckId = Guid.Parse("64b458e1-be36-4f42-86b0-63f7a9ca1503");

        var repo = new SlideDeckReadRepository(repoPath);
        var result = repo.GetSlideDeck(slideDeckId);

        Assert.Equal(slideDeckId, result.Id);
        Assert.Equal("Test Deck 1", result.Title);
        Assert.Equal("A Test of Liquid Victor", result.SubTitle);
        Assert.Equal("Barry S. Stahl", result.Presenter);
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
        Assert.Equal(["character", "emotion", "overworked"], markdown.Tags);
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

    [Fact]
    [Trait("Category", "Integration")]
    public void ReturnTheSampleSlideDeckWithSlideGroups()
    {
        var repoPath = Path.Combine(AppContext.BaseDirectory, "TestRepo");
        var slideDeckId = Guid.Parse("96e4a094-bcee-4622-83ab-42833a103e21");

        var repo = new SlideDeckReadRepository(repoPath);
        var result = repo.GetSlideDeck(slideDeckId);

        Assert.Equal(slideDeckId, result.Id);
        Assert.Equal("Test Deck 2", result.Title);
        Assert.Equal("A Test of Liquid Victor Include Groups", result.SubTitle);
        Assert.Equal("Doc Grouper", result.Presenter);
        Assert.Equal("black", result.ThemeName);
        Assert.Equal("Widescreen", result.AspectRatio.ToString());
        Assert.Equal("Slide", result.Transition.ToString());
        Assert.Equal("https://example.com/TestDeck2", result.SlideDeckUrl?.ToString());

        // Include Block 21d8ebda-d9cc-4c21-9ab3-42e5578ffbb8

        var firstSlide = result.Slides.First().Value;
        Assert.Equal("2facb392-4d95-4c48-abaf-00e97313b9cd", firstSlide.Id.ToString());
        Assert.Equal("8797e86c", firstSlide.Title);
        Assert.Equal("ImageRight", firstSlide.Layout.ToString());
        Assert.Equal("Slide", firstSlide.TransitionIn.ToString());
        Assert.Equal("Fade", firstSlide.TransitionOut.ToString());
        Assert.Null(firstSlide.BackgroundContent);
        Assert.Equal("c47e8653", firstSlide.Notes);
        Assert.False(firstSlide.NeverFullScreen);

        var slide1ContentItem1 = firstSlide.ContentItems.First().Value;
        Assert.Equal("c5d719a5-bdcf-4952-916d-0d15f2e44362", slide1ContentItem1.Id.ToString());
        Assert.Equal("text/markdown", slide1ContentItem1.ContentType);
        Assert.Equal("85b28083", slide1ContentItem1.Title);
        Assert.Equal(8, slide1ContentItem1.Content.Length);
        Assert.Null(slide1ContentItem1.FileName);
        Assert.Null(slide1ContentItem1.Alignment);

        var secondSlide = result.Slides.Skip(1).First().Value;
        Assert.Equal("c80c141c-a930-4429-8d05-daed58d4c7c5", secondSlide.Id.ToString());
        Assert.Equal("Test Slide 2", secondSlide.Title);
        Assert.Equal("FullPageFragments", secondSlide.Layout.ToString());
        Assert.Equal("PresentationDefault", secondSlide.TransitionIn.ToString());
        Assert.Equal("PresentationDefault", secondSlide.TransitionOut.ToString());
        Assert.Null(secondSlide.BackgroundContent);
        Assert.Equal("My 2nd Test Slide", secondSlide.Notes);
        Assert.False(secondSlide.NeverFullScreen);

        var slide2ContentItem1 = secondSlide.ContentItems.First().Value;
        Assert.Equal("187957ad-8831-4498-89ce-fcd76ecc26aa", slide2ContentItem1.Id.ToString());
        Assert.Equal("image/jpg", slide2ContentItem1.ContentType);
        Assert.Equal("Manifest Asynchrony", slide2ContentItem1.Title);
        Assert.Equal(304769, slide2ContentItem1.Content.Length);
        Assert.Equal("Manifest Asynchrony.jpeg", slide2ContentItem1.FileName);
        Assert.Empty(slide2ContentItem1.Alignment);

        var slide2ContentItem2 = secondSlide.ContentItems.Skip(1).First().Value;
        Assert.Equal("4f1e0289-fb19-42a5-9cc1-9d343cc8ee12", slide2ContentItem2.Id.ToString());
        Assert.Equal("text/markdown", slide2ContentItem2.ContentType);
        Assert.Empty(slide2ContentItem2.Title);
        Assert.Equal(103, slide2ContentItem2.Content.Length);
        Assert.Null(slide2ContentItem2.FileName);
        Assert.Empty(slide2ContentItem2.Alignment);

        var thirdSlide = result.Slides.Skip(2).First().Value;
        Assert.Equal("b07300ed-3527-471d-9f36-a9758e545d87", thirdSlide.Id.ToString());
        Assert.Equal("8675309A", thirdSlide.Title);
        Assert.Equal("ImageRight", thirdSlide.Layout.ToString());
        Assert.Equal("Fade", thirdSlide.TransitionIn.ToString());
        Assert.Equal("Slide", thirdSlide.TransitionOut.ToString());
        Assert.Null(thirdSlide.BackgroundContent);
        Assert.Equal("Tommy Tutone!", thirdSlide.Notes);
        Assert.True(thirdSlide.NeverFullScreen);

        var slide3ContentItem1 = thirdSlide.ContentItems.First().Value;
        Assert.Equal("c5d719a5-bdcf-4952-916d-0d15f2e44362", slide3ContentItem1.Id.ToString());
        Assert.Equal("text/markdown", slide3ContentItem1.ContentType);
        Assert.Equal("85b28083", slide3ContentItem1.Title);
        Assert.Equal(8, slide3ContentItem1.Content.Length);
        Assert.Null(slide3ContentItem1.FileName);
        Assert.Null(slide3ContentItem1.Alignment);

        // Slide Include 1b7c1c6b-1d43-46ee-87dc-dc9f5fbd6e5b
        var fourthSlide = result.Slides.Skip(3).First().Value;
        Assert.Equal("1b7c1c6b-1d43-46ee-87dc-dc9f5fbd6e5b", fourthSlide.Id.ToString());
        Assert.Equal("8797e86c", fourthSlide.Title);
        Assert.Equal("ImageRight", fourthSlide.Layout.ToString());
        Assert.Equal("Slide", fourthSlide.TransitionIn.ToString());
        Assert.Equal("Fade", fourthSlide.TransitionOut.ToString());
        Assert.Null(fourthSlide.BackgroundContent);
        Assert.Equal("This is the note", fourthSlide.Notes);
        Assert.False(fourthSlide.NeverFullScreen);

        var slide4ContentItem1 = fourthSlide.ContentItems.First().Value;
        Assert.Equal("c7bfcf44-d87f-4724-a659-7e5afe4d8687", slide4ContentItem1.Id.ToString());
        Assert.Equal("text/markdown", slide4ContentItem1.ContentType);
        Assert.Equal("85b28083", slide4ContentItem1.Title);
        Assert.Equal(20, slide4ContentItem1.Content.Length);
        Assert.Null(slide4ContentItem1.FileName);
        Assert.Null(slide4ContentItem1.Alignment);

        // Single Slide Include Block f7c791f5-36f9-46e0-92dc-97af0e23dd38
        var fifthSlide = result.Slides.Skip(4).First().Value;
        Assert.Equal("833f8eae-471f-4f6d-9493-eb18dd6d4f5e", fifthSlide.Id.ToString());
        Assert.Equal("Test Slide 1", fifthSlide.Title);
        Assert.Equal("ImageLeft", fifthSlide.Layout.ToString());
        Assert.Equal("PresentationDefault", fifthSlide.TransitionIn.ToString());
        Assert.Equal("PresentationDefault", fifthSlide.TransitionOut.ToString());
        Assert.Null(fifthSlide.BackgroundContent);
        Assert.Equal("My 1st Test Slide", fifthSlide.Notes);
        Assert.False(fifthSlide.NeverFullScreen);

        var slide5ContentItem1 = fifthSlide.ContentItems.First().Value;
        Assert.Equal("187957ad-8831-4498-89ce-fcd76ecc26aa", slide5ContentItem1.Id.ToString());
        Assert.Equal("image/jpg", slide5ContentItem1.ContentType);
        Assert.Equal("Manifest Asynchrony", slide5ContentItem1.Title);
        Assert.Equal(304769, slide5ContentItem1.Content.Length);
        Assert.Equal("Manifest Asynchrony.jpeg", slide5ContentItem1.FileName);
        Assert.Empty(slide5ContentItem1.Alignment);

        var slide5ContentItem2 = fifthSlide.ContentItems.Skip(1).First().Value;
        Assert.Equal("4f1e0289-fb19-42a5-9cc1-9d343cc8ee12", slide5ContentItem2.Id.ToString());
        Assert.Equal("text/markdown", slide5ContentItem2.ContentType);
        Assert.Empty(slide5ContentItem2.Title);
        Assert.Equal(103, slide5ContentItem2.Content.Length);
        Assert.Null(slide5ContentItem2.FileName);
        Assert.Empty(slide5ContentItem2.Alignment);
    }
}
