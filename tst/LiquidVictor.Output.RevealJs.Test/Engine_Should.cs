using LiquidVictor.Builders;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Output.RevealJs.Entities;
using LiquidVictor.Output.RevealJs.Generator;

namespace LiquidVictor.Output.RevealJs.Test;

public class Engine_Should
{
    [Fact]
    [Trait("Category", "Integration")]
    public void BuildResourcesSlideAndWriteQrImageWhenEnabled()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        var templatePath = Path.Combine(tempRoot, "template");
        var outputPath = Path.Combine(tempRoot, "output");
        Directory.CreateDirectory(templatePath);
        File.WriteAllText(Path.Combine(templatePath, "index.html"), "{SlideSections}");

        try
        {
            var slideDeck = new SlideDeck { SlideDeckUrl = new Uri("https://example.com/deck") };
            slideDeck.Resources.Add(new Resource { Name = "Guide", Url = "https://example.com/guide" });

            var engine = new Engine(templatePath, new BuilderOptions { BuildTitleSlide = false });
            engine.CreatePresentation(outputPath, slideDeck);

            var html = File.ReadAllText(Path.Combine(outputPath, "index.html"));
            Assert.Contains("Resources", html);
            Assert.Contains("Guide", html);
            var qrImage = Assert.Single(Directory.EnumerateFiles(Path.Combine(outputPath, "img")));
            Assert.EndsWith(".png", qrImage, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(new byte[] { 0x89, 0x50, 0x4e, 0x47 }, File.ReadAllBytes(qrImage).Take(4));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SkipResourcesSlideWhenDisabledOrResourcesAreEmpty()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        var templatePath = Path.Combine(tempRoot, "template");
        var outputPath = Path.Combine(tempRoot, "output");
        Directory.CreateDirectory(templatePath);
        File.WriteAllText(Path.Combine(templatePath, "index.html"), "{SlideSections}");

        try
        {
            var slideDeck = new SlideDeck();
            var engine = new Engine(templatePath, new BuilderOptions
            {
                BuildTitleSlide = false,
                BuildResourcesSlide = true
            });
            engine.CreatePresentation(outputPath, slideDeck);
            Assert.DoesNotContain("Resources", File.ReadAllText(Path.Combine(outputPath, "index.html")));

            slideDeck.Resources.Add(new Resource { Name = "Guide", Url = "https://example.com/guide" });
            var disabledOutputPath = Path.Combine(tempRoot, "disabled-output");
            var disabledEngine = new Engine(templatePath, new BuilderOptions
            {
                BuildTitleSlide = false,
                BuildResourcesSlide = false
            });
            disabledEngine.CreatePresentation(disabledOutputPath, slideDeck);
            Assert.DoesNotContain("Resources", File.ReadAllText(Path.Combine(disabledOutputPath, "index.html")));
            Assert.Empty(Directory.EnumerateFiles(Path.Combine(disabledOutputPath, "img")));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }

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
}
