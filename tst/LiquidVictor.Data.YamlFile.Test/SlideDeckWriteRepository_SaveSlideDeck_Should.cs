using LiquidVictor.Enumerations;
using TestHelperExtensions;
using LiquidVictor.Builders;
using LiquidVictor.Data.Test.Extensions;
using LiquidVictor.Exceptions;
using System.Text;

namespace LiquidVictor.Data.YamlFile.Test;

public class SlideDeckWriteRepository_SaveSlideDeck_Should
{
    [Fact]
    [Trait("Category", "Integration")]
    public void WriteTheSampleSlideDeck()
    {
        var repoPath = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(repoPath, "SlideDecks"));
        Directory.CreateDirectory(Path.Combine(repoPath, "Slides"));
        Directory.CreateDirectory(Path.Combine(repoPath, "ContentItems"));

        try
        {
            var slideDeckId = Guid.NewGuid();
            var slideId = Guid.NewGuid();
            var markdownId = Guid.NewGuid();
            var imageId = Guid.NewGuid();
            var expectedSlide = new SlideBuilder()
                .Id(slideId)
                .Title("Round Trip Slide")
                .Layout(Layout.ImageRight)
                .TransitionIn(Transition.Slide)
                .TransitionOut(Transition.Fancy)
                .Notes("Round trip notes")
                .NeverFullScreen(true)
                .ContentItems(new ContentItemsBuilder()
                    .Add(new ContentItemBuilder()
                        .Id(markdownId)
                        .ContentType("text/markdown")
                        .Title("Markdown")
                        .Content("Line 1" + Environment.NewLine + "Line 2"))
                    .Add(new ContentItemBuilder()
                        .Id(imageId)
                        .ContentType("image/png")
                        .FileName("diagram.png")
                        .Title("Diagram")
                        .Content([1, 2, 3, 4])));

            var slideDeck = new SlideDeckBuilder()
                .Id(slideDeckId)
                .Title("Round Trip Test Deck")
                .SubTitle("YAML")
                .Presenter("Test Presenter")
                .ThemeName("moon")
                .PrintLinkText("Print this deck")
                .AspectRatio(AspectRatio.Standard)
                .Transition(Transition.Fade)
                .SlideDeckUrl("https://example.com/round-trip")
                .Slides(new SlidesBuilder()
                    .Add(expectedSlide))
                .Build();

            var writeRepo = new SlideDeckWriteRepository(repoPath);
            writeRepo.SaveSlideDeck(slideDeck);

            Assert.Single(Directory.EnumerateFiles(Path.Combine(repoPath, "SlideDecks"), "*.yaml"));
            Assert.Single(Directory.EnumerateFiles(Path.Combine(repoPath, "Slides"), "*.yaml"));
            Assert.Equal(2, Directory.EnumerateFiles(Path.Combine(repoPath, "ContentItems"), "*.yaml").Count());

            var readRepo = new SlideDeckReadRepository(repoPath);
            var result = readRepo.GetSlideDeck(slideDeckId);

            Assert.Equal(slideDeckId, result.Id);
            Assert.Equal("Round Trip Test Deck", result.Title);
            Assert.Equal("YAML", result.SubTitle);
            Assert.Equal("Test Presenter", result.Presenter);
            Assert.Equal("moon", result.ThemeName);
            Assert.Equal("Print this deck", result.PrintLinkText);
            Assert.Equal(new Uri("https://example.com/round-trip"), result.SlideDeckUrl);
            Assert.Equal(AspectRatio.Standard, result.AspectRatio);
            Assert.Equal(Transition.Fade, result.Transition);

            var slide = Assert.Single(result.Slides).Value;
            Assert.Equal(slideId, slide.Id);
            Assert.Equal("Round Trip Slide", slide.Title);
            Assert.Equal(Layout.ImageRight, slide.Layout);
            Assert.Equal(Transition.Slide, slide.TransitionIn);
            Assert.Equal(Transition.Fancy, slide.TransitionOut);
            Assert.Equal("Round trip notes", slide.Notes);
            Assert.True(slide.NeverFullScreen);

            var contentItems = slide.ContentItems.Select(ci => ci.Value).ToDictionary(ci => ci.Id);
            Assert.Equal(2, contentItems.Count);

            Assert.True(contentItems.TryGetValue(markdownId, out var markdown));
            Assert.Equal("text/markdown", markdown.ContentType);
            Assert.Equal("Markdown", markdown.Title);
            Assert.Equal("Line 1" + Environment.NewLine + "Line 2", Encoding.UTF8.GetString(markdown.Content));

            Assert.True(contentItems.TryGetValue(imageId, out var image));
            Assert.Equal("image/png", image.ContentType);
            Assert.Equal("diagram.png", image.FileName);
            Assert.Equal("Diagram", image.Title);
            Assert.Equal([1, 2, 3, 4], image.Content);
        }
        finally
        {
            if (Directory.Exists(repoPath))
                Directory.Delete(repoPath, recursive: true);
        }
    }

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
