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
            var deckBackgroundId = Guid.NewGuid();
            var expectedSlide = new SlideBuilder()
                .Id(slideId)
                .Title("Round Trip Slide")
                .Layout(Layout.ImageRight)
                .TransitionIn(Transition.Slide)
                .TransitionOut(Transition.Fancy)
                .BackgroundTransitionIn(Transition.Fade)
                .BackgroundTransitionOut(Transition.Slide)
                .Notes("Round trip notes")
                .NeverFullScreen(true)
                .ContentItems(new ContentItemsBuilder()
                    .Add(new ContentItemBuilder()
                        .Id(markdownId)
                        .ContentType("text/markdown")
                        .Title("Markdown")
                        .Tags(["character", "emotion", "overworked"])
                        .Content("Line 1" + Environment.NewLine + "Line 2"))
                    .Add(new ContentItemBuilder()
                        .Id(imageId)
                        .ContentType("image/png")
                        .FileName("diagram.png")
                        .Title("Diagram")
                        .Tags(["diagram"])
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
                .BackgroundTransition(Transition.Fancy)
                .BackgroundContent(new ContentItemBuilder()
                    .Id(deckBackgroundId)
                    .ContentType("image/png")
                    .FileName("deck-background.png")
                    .Title("Deck Background")
                    .Tags(["deck", "background"])
                    .Content([9, 8, 7, 6]))
                .SlideDeckUrl("https://example.com/round-trip")
                .Slides(new SlidesBuilder()
                    .Add(expectedSlide))
                .Build();
            slideDeck.Resources.Add(new LiquidVictor.Entities.Resource
            {
                Name = "API Guide",
                Type = "Documentation",
                Url = "https://example.com/api"
            });
            slideDeck.Resources.Add(new LiquidVictor.Entities.Resource
            {
                Name = "Community",
                Url = "https://example.com/community"
            });

            var writeRepo = new SlideDeckWriteRepository(repoPath);
            writeRepo.SaveSlideDeck(slideDeck);

            Assert.Single(Directory.EnumerateFiles(Path.Combine(repoPath, "SlideDecks"), "*.yaml"));
            Assert.Single(Directory.EnumerateFiles(Path.Combine(repoPath, "Slides"), "*.yaml"));
            Assert.Equal(3, Directory.EnumerateFiles(Path.Combine(repoPath, "ContentItems"), "*.yaml").Count());

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
            Assert.Equal(Transition.Fancy, result.BackgroundTransition);
            Assert.Collection(result.Resources,
                resource =>
                {
                    Assert.Equal("API Guide", resource.Name);
                    Assert.Equal("Documentation", resource.Type);
                    Assert.Equal("https://example.com/api", resource.Url);
                },
                resource =>
                {
                    Assert.Equal("Community", resource.Name);
                    Assert.Equal("Other", resource.Type);
                    Assert.Equal("https://example.com/community", resource.Url);
                });

            writeRepo.SaveSlideDeck(result);
            var reloadedResult = readRepo.GetSlideDeck(slideDeckId);
            Assert.Equal(
                result.Resources.Select(resource => (resource.Name, resource.Type, resource.Url)),
                reloadedResult.Resources.Select(resource => (resource.Name, resource.Type, resource.Url)));

            Assert.NotNull(result.BackgroundContent);
            Assert.Equal(deckBackgroundId, result.BackgroundContent!.Id);
            Assert.Equal("image/png", result.BackgroundContent.ContentType);
            Assert.Equal("deck-background.png", result.BackgroundContent.FileName);
            Assert.Equal("Deck Background", result.BackgroundContent.Title);
            Assert.Equal(["deck", "background"], result.BackgroundContent.Tags);
            Assert.Equal([9, 8, 7, 6], result.BackgroundContent.Content);

            var slide = Assert.Single(result.Slides).Value;
            Assert.Equal(slideId, slide.Id);
            Assert.Equal("Round Trip Slide", slide.Title);
            Assert.Equal(Layout.ImageRight, slide.Layout);
            Assert.Equal(Transition.Slide, slide.TransitionIn);
            Assert.Equal(Transition.Fancy, slide.TransitionOut);
            Assert.Equal(Transition.Fade, slide.BackgroundTransitionIn);
            Assert.Equal(Transition.Slide, slide.BackgroundTransitionOut);
            Assert.Equal("Round trip notes", slide.Notes);
            Assert.True(slide.NeverFullScreen);

            var contentItems = slide.ContentItems.Select(ci => ci.Value).ToDictionary(ci => ci.Id);
            Assert.Equal(2, contentItems.Count);

            Assert.True(contentItems.TryGetValue(markdownId, out var markdown));
            Assert.Equal("text/markdown", markdown.ContentType);
            Assert.Equal("Markdown", markdown.Title);
            Assert.Equal(["character", "emotion", "overworked"], markdown.Tags);
            Assert.Equal("Line 1" + Environment.NewLine + "Line 2", Encoding.UTF8.GetString(markdown.Content));

            Assert.True(contentItems.TryGetValue(imageId, out var image));
            Assert.Equal("image/png", image.ContentType);
            Assert.Equal("diagram.png", image.FileName);
            Assert.Equal("Diagram", image.Title);
            Assert.Equal(["diagram"], image.Tags);
            Assert.Equal([1, 2, 3, 4], image.Content);
        }
        finally
        {
            if (Directory.Exists(repoPath))
                Directory.Delete(repoPath, recursive: true);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SaveAndRemoveAnOccurrence_WhenTheSameSlideIsUsedMoreThanOnce()
    {
        var deckId = Guid.NewGuid();
        var slideId = Guid.NewGuid();
        var contentItemId = Guid.NewGuid();
        var repoPath = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());

        var slide = new SlideBuilder()
            .Id(slideId)
            .Title("Repeated Slide")
            .Layout(Layout.FullPage)
            .ContentItems(new ContentItemBuilder()
                .Id(contentItemId)
                .ContentType("text/markdown")
                .Title("Slide Content")
                .Content("Content used by both slide occurrences"));
        var repo = new SlideDeckWriteRepository(repoPath);

        try
        {
            var slideDeck = new SlideDeckBuilder()
                .Id(deckId)
                .Title("Repeated Slide Test")
                .Slides(new SlidesBuilder()
                    .Add(slide)
                    .Add(slide))
                .Build();

            repo.SaveSlideDeck(slideDeck);

            Assert.Single(Directory.EnumerateFiles(Path.Combine(repoPath, "Slides"), "*.yaml"));
            Assert.Single(Directory.EnumerateFiles(Path.Combine(repoPath, "ContentItems"), "*.yaml"));

            var readRepo = new SlideDeckReadRepository(repoPath);
            var loadedDeck = readRepo.GetSlideDeck(deckId);
            var occurrences = loadedDeck.Slides.OrderBy(s => s.Key).Select(s => s.Value).ToArray();
            Assert.Equal(2, occurrences.Length);
            Assert.All(occurrences, occurrence =>
            {
                Assert.Equal(slideId, occurrence.Id);
                Assert.Equal("Repeated Slide", occurrence.Title);
                Assert.Equal(contentItemId, Assert.Single(occurrence.ContentItems).Value.Id);
            });

            var updatedDeck = new SlideDeckBuilder()
                .Id(deckId)
                .Title("Repeated Slide Test")
                .Slides(new[] { occurrences[0] })
                .Build();

            repo.SaveSlideDeck(updatedDeck);

            var updatedOccurrences = readRepo.GetSlideDeck(deckId).Slides;
            var remainingOccurrence = Assert.Single(updatedOccurrences).Value;
            Assert.Equal(slideId, remainingOccurrence.Id);
            Assert.Equal("Repeated Slide", remainingOccurrence.Title);
            Assert.Equal(contentItemId, Assert.Single(remainingOccurrence.ContentItems).Value.Id);
            Assert.Single(Directory.EnumerateFiles(Path.Combine(repoPath, "Slides"), "*.yaml"));
        }
        finally
        {
            if (Directory.Exists(repoPath))
                Directory.Delete(repoPath, recursive: true);
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowDuplicateEntityIdException_WhenDifferentSlidesShareAnId()
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
