using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Input.Powerpoint.Test.Builders;
using Microsoft.Extensions.Logging;

namespace LiquidVictor.Input.Powerpoint.Test;

public class SlideDeckImporter_Import_Should
{
    private const string _sourceName = "Sample Deck.pptx";
    private static readonly byte[] _pngBytes = [137, 80, 78, 71, 13, 10, 26, 10, 1, 2, 3, 4];

    [Fact]
    [Trait("Category", "Unit")]
    public void CreateOneSlidePerPowerpointSlideInOrder()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("First"))
            .AddSlide(s => s.Title("Second"))
            .AddSlide(s => s.Title("Third")));

        var titles = slideDeck.Slides.OrderBy(s => s.Key).Select(s => s.Value.Title);
        Assert.Equal(["First", "Second", "Third"], titles);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AssignUniqueIdsToAllEntities()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("One").Body("Point").Picture(_pngBytes))
            .AddSlide(s => s.Title("Two").Body("Point").Picture(_pngBytes)));

        var slideIds = slideDeck.Slides.Select(s => s.Value.Id).ToList();
        var contentItemIds = slideDeck.Slides.SelectMany(s => s.Value.ContentItems).Select(c => c.Value.Id).ToList();

        Assert.NotEqual(Guid.Empty, slideDeck.Id);
        Assert.DoesNotContain(Guid.Empty, slideIds);
        Assert.DoesNotContain(Guid.Empty, contentItemIds);
        Assert.Equal(slideIds.Count, slideIds.Distinct().Count());
        Assert.Equal(4, contentItemIds.Distinct().Count());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void UseTheCenteredTitlePlaceholderAsTheSlideTitle()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.CenteredTitle("Welcome").SubTitle("A subtitle")));

        var slide = slideDeck.Slides.Single().Value;
        Assert.Equal("Welcome", slide.Title);
        Assert.Equal("A subtitle", slide.ContentItems.Single().Value.Content.AsString());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void UseTheSlideNumberAsTheTitleWhenNoTitlePlaceholderExists()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("First"))
            .AddSlide(s => s.TextBox("No title here")));

        Assert.Equal("Slide 2", slideDeck.Slides.Single(s => s.Key == 1).Value.Title);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ConvertContentPlaceholdersToMarkdownBullets()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Bullets").Body((0, "Level one"), (1, "Level two"), (0, "Another"))));

        var contentItem = slideDeck.Slides.Single().Value.ContentItems.Single().Value;
        var expected = string.Join(Environment.NewLine, "- Level one", "  - Level two", "- Another");
        Assert.Equal("text/markdown", contentItem.ContentType);
        Assert.Equal(expected, contentItem.Content.AsString());
        Assert.Equal("Bullets - Text", contentItem.Title);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ConvertTextBoxesToMarkdownParagraphs()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Paragraphs").TextBox("First paragraph", "Second paragraph")));

        var contentItem = slideDeck.Slides.Single().Value.ContentItems.Single().Value;
        var expected = string.Join(Environment.NewLine, "First paragraph", string.Empty, "Second paragraph");
        Assert.Equal(expected, contentItem.Content.AsString());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CreateOneContentItemPerTextShape()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Many").Body("Bullet").TextBox("Box").GroupedTextBox("Grouped")));

        var contentItems = slideDeck.Slides.Single().Value.ContentItems.OrderBy(c => c.Key).Select(c => c.Value).ToList();
        Assert.Equal(["- Bullet", "Box", "Grouped"], contentItems.Select(c => c.Content.AsString()));
        Assert.Equal(["Many - Text 1", "Many - Text 2", "Many - Text 3"], contentItems.Select(c => c.Title));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void IgnoreFooterPlaceholders()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Footer").Footer("Company Confidential")));

        Assert.Empty(slideDeck.Slides.Single().Value.ContentItems);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ImportEmbeddedPicturesAsImageContentItems()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Picture").Picture(_pngBytes, "image/png", "Picture 1", "A diagram")));

        var contentItem = slideDeck.Slides.Single().Value.ContentItems.Single().Value;
        Assert.Equal("image/png", contentItem.ContentType);
        Assert.Equal(_pngBytes, contentItem.Content);
        Assert.Equal("A diagram", contentItem.Title);
        Assert.EndsWith(".png", contentItem.FileName, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void UseThePictureNameAsTheTitleWhenNoAltTextExists()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Picture(_pngBytes, "image/jpeg", "Company Logo")));

        var contentItem = slideDeck.Slides.Single().Value.ContentItems.Single().Value;
        Assert.Equal("Company Logo", contentItem.Title);
        Assert.Equal("image/jpeg", contentItem.ContentType);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PlaceTextBeforeImagesInTheContentItems()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Mixed").Picture(_pngBytes).Body("Text")));

        var contentTypes = slideDeck.Slides.Single().Value.ContentItems.OrderBy(c => c.Key).Select(c => c.Value.ContentType);
        Assert.Equal(["text/markdown", "image/png"], contentTypes);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SkipAndWarnAboutPicturesThatAreNotEmbedded()
    {
        var (slideDeck, logger) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Linked").LinkedPicture("External Image")));

        Assert.Empty(slideDeck.Slides.Single().Value.ContentItems);
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Contains("External Image", StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SkipAndWarnAboutUnsupportedElements()
    {
        var (slideDeck, logger) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Table").Table()));

        Assert.Empty(slideDeck.Slides.Single().Value.ContentItems);
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Contains("graphicFrame", StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SkipAndWarnAboutSlidesThatCannotBeResolved()
    {
        var (slideDeck, logger) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("First"))
            .AddUnresolvableSlide()
            .AddSlide(s => s.Title("Third")));

        Assert.Equal(["First", "Third"], slideDeck.Slides.OrderBy(s => s.Key).Select(s => s.Value.Title));
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Contains("Skipping slide 2", StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ImportSpeakerNotes()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Notes").Notes("Remember this\nAnd this")));

        var expected = string.Join(Environment.NewLine, "Remember this", "And this");
        Assert.Equal(expected, slideDeck.Slides.Single().Value.Notes);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void LeaveNotesEmptyWhenTheSlideHasNoNotes()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("No Notes")));

        Assert.Equal(string.Empty, slideDeck.Slides.Single().Value.Notes);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void TagEveryContentItemWithTheSourceDeck()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("One").Body("Point").Picture(_pngBytes))
            .AddSlide(s => s.Title("Two").TextBox("Box")));

        var contentItems = slideDeck.Slides.SelectMany(s => s.Value.ContentItems).Select(c => c.Value).ToList();
        Assert.Equal(3, contentItems.Count);
        Assert.All(contentItems, c => Assert.Equal(["pptx:Sample Deck.pptx"], c.Tags));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("Text", Layout.FullPage)]
    [InlineData("Image", Layout.FullPage)]
    [InlineData("TextAndImage", Layout.ImageRight)]
    [InlineData("Empty", Layout.FullPage)]
    public void SelectALayoutBasedOnTheSlideContent(string content, Layout expected)
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s =>
            {
                s.Title("Layout");
                if (content.Contains("Text", StringComparison.Ordinal))
                    s.Body("Text");
                if (content.Contains("Image", StringComparison.Ordinal))
                    s.Picture(_pngBytes);
            }));

        Assert.Equal(expected, slideDeck.Slides.Single().Value.Layout);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void UsePresentationDefaultTransitions()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("Transitions")));

        var slide = slideDeck.Slides.Single().Value;
        Assert.Equal(Transition.PresentationDefault, slide.TransitionIn);
        Assert.Equal(Transition.PresentationDefault, slide.TransitionOut);
        Assert.Equal(Transition.PresentationDefault, slide.BackgroundTransitionIn);
        Assert.Equal(Transition.PresentationDefault, slide.BackgroundTransitionOut);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void UseTheDocumentPropertiesForTheSlideDeck()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .Title("Deck Title")
            .Subject("Deck Subject")
            .Creator("Deck Author")
            .AddSlide(s => s.Title("One")));

        Assert.Equal("Deck Title", slideDeck.Title);
        Assert.Equal("Deck Subject", slideDeck.SubTitle);
        Assert.Equal("Deck Author", slideDeck.Presenter);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void UseTheFileNameAsTheTitleWhenNoTitlePropertyExists()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("One")));

        Assert.Equal("Sample Deck", slideDeck.Title);
        Assert.Equal(string.Empty, slideDeck.SubTitle);
        Assert.Equal(string.Empty, slideDeck.Presenter);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(12192000, 6858000, AspectRatio.Widescreen)] // 16:9
    [InlineData(9144000, 5715000, AspectRatio.Widescreen)]  // 16:10
    [InlineData(9144000, 6858000, AspectRatio.Standard)]    // 4:3
    public void DetermineTheAspectRatioFromTheSlideSize(int width, int height, AspectRatio expected)
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .SlideSize(width, height)
            .AddSlide(s => s.Title("One")));

        Assert.Equal(expected, slideDeck.AspectRatio);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DefaultToWidescreenAndWarnWhenTheSlideSizeIsMissing()
    {
        var (slideDeck, logger) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("One")));

        Assert.Equal(AspectRatio.Widescreen, slideDeck.AspectRatio);
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Contains("slide size", StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ImportOnlyTheSelectedSlides()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("One"))
            .AddSlide(s => s.Title("Two"))
            .AddSlide(s => s.Title("Three")),
            [3, 1, 3]);

        Assert.Equal(["One", "Three"], slideDeck.Slides.OrderBy(s => s.Key).Select(s => s.Value.Title));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ImportAllSlidesWhenTheSelectionIsEmpty()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("One"))
            .AddSlide(s => s.Title("Two")),
            []);

        Assert.Equal(2, slideDeck.Slides.Count);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-1)]
    public void ThrowAndLogAnErrorWhenASelectedSlideDoesNotExist(int slideNumber)
    {
        var logger = new ListLogger<SlideDeckImporter>();
        using var stream = new PowerpointFileBuilder()
            .AddSlide(s => s.Title("One"))
            .AddSlide(s => s.Title("Two"))
            .Build();

        var target = new SlideDeckImporter(logger);
        Assert.Throws<ArgumentOutOfRangeException>(() => target.Import(stream, _sourceName, [slideNumber]));
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Error);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnAnEmptySlideDeckForAPresentationWithNoSlides()
    {
        var (slideDeck, _) = Import(new PowerpointFileBuilder());

        Assert.Empty(slideDeck.Slides);
        Assert.Equal("Sample Deck", slideDeck.Title);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void LogTheStartAndCompletionOfTheImport()
    {
        var (slideDeck, logger) = Import(new PowerpointFileBuilder()
            .AddSlide(s => s.Title("One").Body("Text")));

        var information = logger.Entries.Where(e => e.Level == LogLevel.Information).Select(e => e.Message).ToList();
        Assert.Contains(information, m => m.Contains("Importing PowerPoint deck Sample Deck.pptx", StringComparison.Ordinal));
        Assert.Contains(information, m => m.Contains(slideDeck.Id.ToString(), StringComparison.Ordinal)
            && m.Contains("1 slide(s) and 1 content item(s)", StringComparison.Ordinal));
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Trace);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowAndLogAnErrorWhenTheStreamIsNotAPresentation()
    {
        var logger = new ListLogger<SlideDeckImporter>();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is not a PowerPoint file"));

        var target = new SlideDeckImporter(logger);
        Assert.Throws<InvalidDataException>(() => target.Import(stream, _sourceName));
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Error && e.Exception is not null);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowAndLogAnErrorWhenTheFileDoesNotExist()
    {
        var logger = new ListLogger<SlideDeckImporter>();
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pptx");

        var target = new SlideDeckImporter(logger);
        Assert.Throws<FileNotFoundException>(() => target.Import(filePath));
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Error);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void ImportAPowerpointFileFromDisk()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pptx");
        try
        {
            new PowerpointFileBuilder()
                .AddSlide(s => s.Title("From Disk").Body("Point"))
                .BuildFile(filePath);

            var target = new SlideDeckImporter(new ListLogger<SlideDeckImporter>());
            var slideDeck = target.Import(filePath);

            var slide = slideDeck.Slides.Single().Value;
            Assert.Equal(Path.GetFileNameWithoutExtension(filePath), slideDeck.Title);
            Assert.Equal("From Disk", slide.Title);
            Assert.Equal([$"pptx:{Path.GetFileName(filePath)}"], slide.ContentItems.Single().Value.Tags);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowWhenTheLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SlideDeckImporter(null!));
    }

    private static (SlideDeck SlideDeck, ListLogger<SlideDeckImporter> Logger) Import(PowerpointFileBuilder builder, IEnumerable<int>? slideNumbers = null)
    {
        var logger = new ListLogger<SlideDeckImporter>();
        using var stream = builder.Build();
        var slideDeck = new SlideDeckImporter(logger).Import(stream, _sourceName, slideNumbers);
        return (slideDeck, logger);
    }
}

internal static class ByteArrayExtensions
{
    public static string AsString(this byte[] value) => Encoding.UTF8.GetString(value);
}
