using LiquidVictor.Builders;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Output.RevealJs.Entities;
using LiquidVictor.Output.RevealJs.Interfaces;
using Markdig;

namespace LiquidVictor.Output.RevealJs.Test;

public class FragmentLayouts_Should
{
    private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public static TheoryData<Enumerations.Layout> FragmentLayouts => new()
    {
        Enumerations.Layout.FullPageFragments,
        Enumerations.Layout.ImageLeftFragments,
        Enumerations.Layout.ImageRightFragments
    };

    [Theory]
    [MemberData(nameof(FragmentLayouts))]
    public void DisplayTheFirstTextItemWhenTheSlideLoads(Enumerations.Layout layout)
    {
        var html = CreateStrategy(layout).Layout(CreateSlide(layout), 0);

        Assert.Contains("<p>First item</p>", html, StringComparison.Ordinal);
        Assert.DoesNotContain("class=\"fragment\">First item", html, StringComparison.Ordinal);
    }

    [Theory]
    [MemberData(nameof(FragmentLayouts))]
    public void RevealSubsequentTextItemsAsFragments(Enumerations.Layout layout)
    {
        var html = CreateStrategy(layout).Layout(CreateSlide(layout), 0);

        Assert.Contains("<p class=\"fragment\">Second item</p>", html, StringComparison.Ordinal);
        Assert.Contains("<p class=\"fragment\">Third item</p>", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(Enumerations.Layout.ImageLeftFragments)]
    [InlineData(Enumerations.Layout.ImageRightFragments)]
    public void IncludeTheImage(Enumerations.Layout layout)
    {
        var slide = CreateSlide(layout);
        var image = slide.ContentItems.Select(c => c.Value).Single(c => c.ContentType.StartsWith("image", StringComparison.Ordinal));

        var html = CreateStrategy(layout).Layout(slide, 0);

        Assert.Contains($"src=\"img/{image.Id}.png\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public void PlaceTheImageBeforeTheTextForImageLeftFragments()
    {
        var html = CreateStrategy(Enumerations.Layout.ImageLeftFragments).Layout(CreateSlide(Enumerations.Layout.ImageLeftFragments), 0);

        Assert.True(html.IndexOf("<img", StringComparison.Ordinal) < html.IndexOf("First item", StringComparison.Ordinal));
    }

    [Fact]
    public void PlaceTheImageAfterTheTextForImageRightFragments()
    {
        var html = CreateStrategy(Enumerations.Layout.ImageRightFragments).Layout(CreateSlide(Enumerations.Layout.ImageRightFragments), 0);

        Assert.True(html.IndexOf("<img", StringComparison.Ordinal) > html.IndexOf("Third item", StringComparison.Ordinal));
    }

    [Theory]
    [MemberData(nameof(FragmentLayouts))]
    public void BeUsedByTheGenerator(Enumerations.Layout layout)
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
                .ThemeName("moon")
                .PrintLinkText("Print")
                .AspectRatio(AspectRatio.Widescreen)
                .Slides(new SlidesBuilder().Add(CreateSlide(layout)))
                .Build();

            var engine = new Generator.Engine(templatePath, new BuilderOptions
            {
                BuildTitleSlide = false,
                MakeSoloImagesFullScreen = false
            });

            engine.CreatePresentation(outputPath, slideDeck);

            var html = File.ReadAllText(Path.Combine(outputPath, "index.html"));
            Assert.Contains($"<!-- Layout:{layout} -->", html, StringComparison.Ordinal);
            Assert.Contains("<p class=\"fragment\">Second item</p>", html, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }

    private static ILayoutStrategy CreateStrategy(Enumerations.Layout layout)
    {
        var builderOptions = new BuilderOptions();
        return layout switch
        {
            Enumerations.Layout.FullPageFragments => new Layout.FullPageFragments.Engine(_pipeline, Transition.Slide, Transition.Fade, null, builderOptions),
            Enumerations.Layout.ImageLeftFragments => new Layout.ImageLeftFragments.Engine(_pipeline, Transition.Slide, Transition.Fade, null, builderOptions),
            Enumerations.Layout.ImageRightFragments => new Layout.ImageRightFragments.Engine(_pipeline, Transition.Slide, Transition.Fade, null, builderOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(layout))
        };
    }

    private static Slide CreateSlide(Enumerations.Layout layout)
    {
        return new SlideBuilder()
            .Title("Fragment Slide")
            .Layout(layout)
            .ContentItems(new ContentItemBuilder()
                .Title("First")
                .ContentType("text/markdown")
                .Content("First item"))
            .ContentItems(new ContentItemBuilder()
                .Title("Image")
                .FileName("picture.png")
                .ContentType("image/png")
                .Content([1, 2, 3]))
            .ContentItems(new ContentItemBuilder()
                .Title("Second")
                .ContentType("text/markdown")
                .Content("Second item"))
            .ContentItems(new ContentItemBuilder()
                .Title("Third")
                .ContentType("text/markdown")
                .Content("Third item"))
            .Build();
    }
}
