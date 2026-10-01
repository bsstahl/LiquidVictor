using LiquidVictor.Builders;
using LiquidVictor.Enumerations;
using LiquidVictor.Output.RevealJs.Entities;
using LiquidVictor.Output.RevealJs.Generator;

namespace LiquidVictor.Output.RevealJs.Test;

public class Engine_Should
{
    [Fact]
    [Trait("Category", "Integration")]
    public void ApplyDeckBackgroundByDefaultAndAllowSlideOverride()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        var templatePath = Path.Combine(tempRoot, "template");
        var outputPath = Path.Combine(tempRoot, "output");

        Directory.CreateDirectory(templatePath);
        File.WriteAllText(
            Path.Combine(templatePath, "index.html"),
            "<html><body><div class=\"slides\">{SlideSections}</div></body></html>");

        try
        {
            var deckBackgroundId = Guid.NewGuid();
            var overrideBackgroundId = Guid.NewGuid();

            var slideDeck = new SlideDeckBuilder()
                .Title("Deck")
                .SubTitle("Subtitle")
                .Presenter("Presenter")
                .ThemeName("moon")
                .PrintLinkText("Print")
                .AspectRatio(AspectRatio.Widescreen)
                .Transition(Transition.Slide)
                .BackgroundTransition(Transition.Fade)
                .BackgroundContent(new ContentItemBuilder()
                    .Id(deckBackgroundId)
                    .Title("Default Background")
                    .FileName("default.png")
                    .ContentType("image/png")
                    .Content([1, 2, 3]))
                .Slides(new SlidesBuilder()
                    .Add(new SlideBuilder()
                        .Title("Uses Default")
                        .Layout(Enumerations.Layout.ImageRight)
                        .ContentItems(new ContentItemBuilder()
                            .Title("Content")
                            .ContentType("text/markdown")
                            .Content("# Slide 1")))
                    .Add(new SlideBuilder()
                        .Title("Uses Override")
                        .Layout(Enumerations.Layout.ImageRight)
                        .BackgroundContent(new ContentItemBuilder()
                            .Id(overrideBackgroundId)
                            .Title("Override Background")
                            .FileName("override.png")
                            .ContentType("image/png")
                            .Content([4, 5, 6]))
                        .ContentItems(new ContentItemBuilder()
                            .Title("Content")
                            .ContentType("text/markdown")
                            .Content("# Slide 2"))))
                .Build();

            var engine = new Engine(templatePath, new BuilderOptions
            {
                BuildTitleSlide = true,
                MakeSoloImagesFullScreen = false
            });

            engine.CreatePresentation(outputPath, slideDeck);

            var html = File.ReadAllText(Path.Combine(outputPath, "index.html"));
            Assert.Equal(2, CountOccurrences(html, $"data-background='img/{deckBackgroundId}.png'"));
            Assert.Contains($"data-background='img/{overrideBackgroundId}.png'", html);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }

    private static int CountOccurrences(string value, string expected)
    {
        var count = 0;
        var index = 0;
        while ((index = value.IndexOf(expected, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += expected.Length;
        }

        return count;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddIndexSlideAtEndOfDeck_WhenBuildIndexSlideIsTrueAndSectionHeadingsExist()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        var templatePath = Path.Combine(tempRoot, "template");
        var outputPath = Path.Combine(tempRoot, "output");

        Directory.CreateDirectory(templatePath);
        File.WriteAllText(
            Path.Combine(templatePath, "index.html"),
            "<html><body><div class=\"slides\">{SlideSections}</div></body></html>");

        try
        {
            var sectionSlideId = Guid.NewGuid();
            var slideDeck = new SlideDeckBuilder()
                .Title("My Deck")
                .Slides(new SlidesBuilder()
                    .Add(new SlideBuilder()
                        .Id(sectionSlideId)
                        .Title("Introduction")
                        .Layout(Enumerations.Layout.FullPage)
                        .IsSectionHeading(true)
                        .ContentItems(new ContentItemBuilder()
                            .Title("Intro Content")
                            .ContentType("text/markdown")
                            .Content("# Welcome")))
                    .Add(new SlideBuilder()
                        .Title("Regular Slide")
                        .Layout(Enumerations.Layout.FullPage)
                        .IsSectionHeading(false)
                        .ContentItems(new ContentItemBuilder()
                            .Title("Detail")
                            .ContentType("text/markdown")
                            .Content("# Details"))))
                .Build();

            var engine = new Engine(templatePath, new BuilderOptions
            {
                BuildTitleSlide = false,
                BuildIndexSlide = true
            });

            engine.CreatePresentation(outputPath, slideDeck);

            var html = File.ReadAllText(Path.Combine(outputPath, "index.html"));

            // Verify index slide was generated
            Assert.Contains("<h1>Index</h1>", html);
            Assert.Contains($"<a href=\"#{sectionSlideId}\">Introduction</a>", html);

            // Verify index slide is at the end of the presentation (after regular slide)
            var regularIndex = html.IndexOf("Details", StringComparison.Ordinal);
            var indexSlidePos = html.IndexOf("<h1>Index</h1>", StringComparison.Ordinal);
            Assert.True(indexSlidePos > regularIndex, "Index slide should be added to the end of the deck");
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void NotAddIndexSlide_WhenBuildIndexSlideIsFalse()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        var templatePath = Path.Combine(tempRoot, "template");
        var outputPath = Path.Combine(tempRoot, "output");

        Directory.CreateDirectory(templatePath);
        File.WriteAllText(
            Path.Combine(templatePath, "index.html"),
            "<html><body><div class=\"slides\">{SlideSections}</div></body></html>");

        try
        {
            var sectionSlideId = Guid.NewGuid();
            var slideDeck = new SlideDeckBuilder()
                .Title("My Deck")
                .Slides(new SlidesBuilder()
                    .Add(new SlideBuilder()
                        .Id(sectionSlideId)
                        .Title("Introduction")
                        .Layout(Enumerations.Layout.FullPage)
                        .IsSectionHeading(true)
                        .ContentItems(new ContentItemBuilder()
                            .Title("Intro Content")
                            .ContentType("text/markdown")
                            .Content("# Welcome"))))
                .Build();

            var engine = new Engine(templatePath, new BuilderOptions
            {
                BuildTitleSlide = false,
                BuildIndexSlide = false
            });

            engine.CreatePresentation(outputPath, slideDeck);

            var html = File.ReadAllText(Path.Combine(outputPath, "index.html"));

            Assert.DoesNotContain("<h1>Index</h1>", html);
            Assert.DoesNotContain($"href=\"#{sectionSlideId}\"", html);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void NotAddIndexSlide_WhenNoSectionHeadingsExist()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        var templatePath = Path.Combine(tempRoot, "template");
        var outputPath = Path.Combine(tempRoot, "output");

        Directory.CreateDirectory(templatePath);
        File.WriteAllText(
            Path.Combine(templatePath, "index.html"),
            "<html><body><div class=\"slides\">{SlideSections}</div></body></html>");

        try
        {
            var slideDeck = new SlideDeckBuilder()
                .Title("My Deck")
                .Slides(new SlidesBuilder()
                    .Add(new SlideBuilder()
                        .Title("Regular Slide")
                        .Layout(Enumerations.Layout.FullPage)
                        .IsSectionHeading(false)
                        .ContentItems(new ContentItemBuilder()
                            .Title("Detail")
                            .ContentType("text/markdown")
                            .Content("# Details"))))
                .Build();

            var engine = new Engine(templatePath, new BuilderOptions
            {
                BuildTitleSlide = false,
                BuildIndexSlide = true
            });

            engine.CreatePresentation(outputPath, slideDeck);

            var html = File.ReadAllText(Path.Combine(outputPath, "index.html"));

            Assert.DoesNotContain("<h1>Index</h1>", html);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }
}
