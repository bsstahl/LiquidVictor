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

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData(FooterDisplay.Default, 2)]
    [InlineData(FooterDisplay.Always, 0)]
    [InlineData(FooterDisplay.Never, 4)]
    public void HideFootersBasedOnDeckAndSlideSettings(FooterDisplay footerDisplay, int expectedHiddenFooterCount)
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
                .Title("Deck")
                .SubTitle("Subtitle")
                .Presenter("Presenter")
                .PrintLinkText("Print")
                .FooterDisplay(footerDisplay)
                .Slides(new SlidesBuilder()
                    .Add(new SlideBuilder()
                        .Title("Text Slide")
                        .Layout(Enumerations.Layout.FullPage)
                        .ContentItems(new ContentItemBuilder()
                            .Title("Content")
                            .ContentType("text/markdown")
                            .Content("# Slide 1")))
                    .Add(new SlideBuilder()
                        .Layout(Enumerations.Layout.FullPage)
                        .ContentItems(new ContentItemBuilder()
                            .Title("Full Screen Image")
                            .FileName("fullscreen.png")
                            .ContentType("image/png")
                            .Content([1, 2, 3])))
                    .Add(new SlideBuilder()
                        .Title("Text Slide Without Footer")
                        .Layout(Enumerations.Layout.FullPage)
                        .ShowFooter(false)
                        .ContentItems(new ContentItemBuilder()
                            .Title("Content")
                            .ContentType("text/markdown")
                            .Content("# Slide 3"))))
                .Build();

            var engine = new Engine(templatePath, new BuilderOptions
            {
                BuildTitleSlide = true,
                MakeSoloImagesFullScreen = true
            });

            engine.CreatePresentation(outputPath, slideDeck);

            var html = File.ReadAllText(Path.Combine(outputPath, "index.html"));
            Assert.Equal(expectedHiddenFooterCount, CountOccurrences(html, "data-state=\"hide-footer\""));
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
}
