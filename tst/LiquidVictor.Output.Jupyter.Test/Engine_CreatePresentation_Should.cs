using System.Text.Json;
using LiquidVictor.Builders;
using LiquidVictor.Output.Jupyter.Entities;
using LiquidVictor.Output.Jupyter.Generator;

namespace LiquidVictor.Output.Jupyter.Test;

public class Engine_CreatePresentation_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void WriteANotebookToTheOutputDirectory()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputPath);

        try
        {
            var engine = new Engine(new BuilderOptions { BuildTitleSlide = true });
            var slideDeck = new SlideDeckBuilder()
                .Title("Notebook Deck")
                .SubTitle("Subtitle")
                .Presenter("Presenter")
                .ThemeName("Black")
                .Slides(new SlidesBuilder()
                    .Add(new SlideBuilder()
                        .Title("First Slide")
                        .ContentItems(new ContentItemBuilder()
                            .ContentType("text/markdown")
                            .Content("Body markdown"))
                        .ContentItems(new ContentItemBuilder()
                            .ContentType("image/png")
                            .FileName("diagram.png")
                            .Title("Diagram")
                            .Content([1, 2, 3]))))
                .Build();

            engine.CreatePresentation(outputPath, slideDeck);

            var notebookPath = Path.Combine(outputPath, "presentation.ipynb");
            Assert.True(File.Exists(notebookPath));

            using var document = JsonDocument.Parse(File.ReadAllText(notebookPath));
            Assert.Equal(4, document.RootElement.GetProperty("nbformat").GetInt32());
            Assert.Equal(5, document.RootElement.GetProperty("nbformat_minor").GetInt32());

            var cells = document.RootElement.GetProperty("cells");
            Assert.Equal(2, cells.GetArrayLength());
            Assert.Equal("slide", cells[0].GetProperty("metadata").GetProperty("slideshow").GetProperty("slide_type").GetString());
            Assert.Equal("slide", cells[1].GetProperty("metadata").GetProperty("slideshow").GetProperty("slide_type").GetString());
            Assert.Contains("First Slide", string.Join(string.Empty, cells[1].GetProperty("source").EnumerateArray().Select(c => c.GetString())));
            Assert.Contains("data:image/png;base64", string.Join(string.Empty, cells[1].GetProperty("source").EnumerateArray().Select(c => c.GetString())));
            Assert.Equal("Black", document.RootElement.GetProperty("metadata").GetProperty("livereveal").GetProperty("theme").GetString());
        }
        finally
        {
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, recursive: true);
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void WriteANotebookToTheSpecifiedIpynbFile()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
        var notebookPath = Path.Combine(outputPath, "deck.ipynb");
        Directory.CreateDirectory(outputPath);

        try
        {
            var engine = new Engine(new BuilderOptions { BuildTitleSlide = false });
            var slideDeck = new SlideDeckBuilder()
                .Title("Notebook Deck")
                .Slides(new SlidesBuilder()
                    .Add(new SlideBuilder()
                        .Title("First Slide")
                        .ContentItems(new ContentItemBuilder()
                            .ContentType("text/plain")
                            .Content("Body text"))))
                .Build();

            engine.CreatePresentation(notebookPath, slideDeck);

            Assert.True(File.Exists(notebookPath));
            using var document = JsonDocument.Parse(File.ReadAllText(notebookPath));
            Assert.Single(document.RootElement.GetProperty("cells").EnumerateArray());
        }
        finally
        {
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, recursive: true);
        }
    }
}
