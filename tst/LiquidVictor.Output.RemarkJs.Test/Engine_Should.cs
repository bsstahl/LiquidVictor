using LiquidVictor.Builders;
using LiquidVictor.Enumerations;
using LiquidVictor.Output.RemarkJs.Generator;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.RegularExpressions;

namespace LiquidVictor.Output.RemarkJs.Test;

public class Engine_Should
{
    [Fact]
    public void CreateRemarkPresentationWithSlidesNotesAndImages()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        var imageId = Guid.NewGuid();
        var backgroundId = Guid.NewGuid();
        var slideDeck = new SlideDeckBuilder()
            .Title("A deck")
            .SubTitle("A subtitle")
            .Presenter("A presenter")
            .ThemeName("black")
            .PrintLinkText("Print")
            .AspectRatio(AspectRatio.Widescreen)
            .Transition(Transition.Slide)
            .BackgroundContent(new ContentItemBuilder()
                .Id(backgroundId)
                .FileName("background.png")
                .ContentType("image/png")
                .Content([4, 5, 6]))
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder()
                    .Title("A slide")
                    .Notes("Speaker notes")
                    .ContentItems(new ContentItemBuilder()
                        .ContentType("text/markdown")
                        .Content("A **markdown** body"))
                    .ContentItems(new ContentItemBuilder()
                        .Id(imageId)
                        .FileName("photo.png")
                        .ContentType("image/png")
                        .Content([1, 2, 3]))))
            .Build();

        try
        {
            var engine = new Engine(NullLogger<Engine>.Instance, buildTitleSlide: true);
            engine.CreatePresentation(outputPath, slideDeck);

            var html = File.ReadAllText(Path.Combine(outputPath, "index.html"));
            Assert.Contains("remark.create()", html);
            Assert.Contains("Content-Security-Policy", html);
            Assert.Contains("# A deck", html);
            Assert.Contains("# A slide", html);
            Assert.Contains("A **markdown** body", html);
            Assert.Contains("???", html);
            Assert.Contains("Speaker notes", html);
            Assert.Contains($"img/{imageId:D}.png", html);
            Assert.Contains($"background-image: url(img/{backgroundId:D}.png)", html);
            Assert.Equal([1, 2, 3], File.ReadAllBytes(Path.Combine(outputPath, "img", $"{imageId:D}.png")));
            Assert.Equal([4, 5, 6], File.ReadAllBytes(Path.Combine(outputPath, "img", $"{backgroundId:D}.png")));
        }
        finally
        {
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, recursive: true);
        }
    }

    [Fact]
    public void EncodeMarkdownThatCouldCloseTheSourceTextarea()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        var slideDeck = new SlideDeckBuilder()
            .Title("A deck")
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder()
                    .ContentItems(new ContentItemBuilder()
                        .ContentType("text/markdown")
                        .Content("</textarea><script>alert(1)</script>"))))
            .Build();

        try
        {
            new Engine(NullLogger<Engine>.Instance, buildTitleSlide: false)
                .CreatePresentation(outputPath, slideDeck);

            var html = File.ReadAllText(Path.Combine(outputPath, "index.html"));
            Assert.Contains("&lt;/textarea&gt;&lt;script&gt;alert(1)&lt;/script&gt;", html);
            Assert.Equal(1, html.Split("</textarea>", StringSplitOptions.None).Length - 1);
            var nonce = Regex.Match(html, """<script nonce="([^"]+)">remark\.create\(\);</script>""").Groups[1].Value;
            Assert.NotEmpty(nonce);
            Assert.Contains($"script-src 'nonce-{nonce}' https://remarkjs.com", html);
        }
        finally
        {
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, recursive: true);
        }
    }
}
