using LiquidVictor.Entities;
using LiquidVictor.Input.Powerpoint;
using LiquidVictor.Input.Powerpoint.Test.Builders;
using LiquidVictor.Interfaces;
using Microsoft.Extensions.Logging;

namespace LVImport.Test;

public sealed class ImportCommand_Execute_Should : IDisposable
{
    private static readonly byte[] _pngBytes = [137, 80, 78, 71, 13, 10, 26, 10, 5, 6, 7, 8];

    private readonly string _workingPath = Path.Combine(Path.GetTempPath(), "LiquidVictor", Guid.NewGuid().ToString());
    private readonly string _powerpointPath;

    public ImportCommand_Execute_Should()
    {
        Directory.CreateDirectory(_workingPath);
        _powerpointPath = new PowerpointFileBuilder()
            .Title("Imported Deck")
            .Creator("Presenter Name")
            .AddSlide(s => s.CenteredTitle("Welcome").SubTitle("An imported deck").Notes("Say hello"))
            .AddSlide(s => s.Title("Agenda").Body((0, "Intro"), (1, "Details")).Picture(_pngBytes, "image/png", "Picture 1", "Diagram"))
            .AddSlide(s => s.Title("Wrap Up").TextBox("Thanks!"))
            .BuildFile(Path.Combine(_workingPath, "Source Deck.pptx"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_workingPath))
            Directory.Delete(_workingPath, true);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SaveTheImportedSlideDeckToTheRepository()
    {
        var repository = new FakeWriteRepository();
        var target = CreateTarget(out _);

        var result = target.Execute(new ImportOptions() { PowerpointPath = _powerpointPath, SourceRepoPath = "unused" }, repository);

        var saved = Assert.Single(repository.SavedSlideDecks);
        Assert.Same(result, saved);
        Assert.Equal(3, saved.Slides.Count);
        Assert.Equal("Imported Deck", saved.Title);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ImportOnlyTheSelectedSlides()
    {
        var repository = new FakeWriteRepository();
        var target = CreateTarget(out _);

        var result = target.Execute(new ImportOptions() { PowerpointPath = _powerpointPath, SlideNumbers = [1, 3] }, repository);

        Assert.Equal(["Welcome", "Wrap Up"], result.Slides.OrderBy(s => s.Key).Select(s => s.Value.Title));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void OverrideTheTitleWhenSpecified()
    {
        var repository = new FakeWriteRepository();
        var target = CreateTarget(out _);

        var result = target.Execute(new ImportOptions() { PowerpointPath = _powerpointPath, Title = "New Title" }, repository);

        Assert.Equal("New Title", result.Title);
        Assert.Equal("New Title", repository.SavedSlideDecks.Single().Title);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NotSaveTheSlideDeckWhenOutputIsSkipped()
    {
        var target = CreateTarget(out var logger);

        var result = target.Execute(new ImportOptions() { PowerpointPath = _powerpointPath, SkipOutput = true }, null);

        Assert.Equal(3, result.Slides.Count);
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Information && e.Message.Contains("not saved", StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowWhenNoRepositoryIsProvidedAndOutputIsNotSkipped()
    {
        var target = CreateTarget(out _);
        Assert.Throws<ArgumentNullException>(() => target.Execute(new ImportOptions() { PowerpointPath = _powerpointPath }, null));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void LogTheSaveOperation()
    {
        var target = CreateTarget(out var logger);

        var result = target.Execute(new ImportOptions() { PowerpointPath = _powerpointPath }, new FakeWriteRepository());

        var information = logger.Entries.Where(e => e.Level == LogLevel.Information).Select(e => e.Message).ToList();
        Assert.Contains(information, m => m.Contains("Starting PowerPoint import", StringComparison.Ordinal));
        Assert.Contains(information, m => m.Contains($"Saved slide deck {result.Id}", StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void WriteAnImportedDeckThatCanBeReadFromAYamlFileRepository()
    {
        var repoPath = Path.Combine(_workingPath, "Repo");
        var options = ImportArguments.Parse([_powerpointPath, "-SourceRepoType:YamlFile", $"-SourceRepoPath:{repoPath}"]);
        var writeRepository = WriteRepositoryFactory.Create(options.SourceRepoType, options.SourceRepoPath);
        var target = CreateTarget(out _);

        var imported = target.Execute(options, writeRepository);

        var readRepository = new LiquidVictor.Data.YamlFile.SlideDeckReadRepository(repoPath);
        var actual = readRepository.GetSlideDeck(imported.Id);
        var slides = actual.Slides.OrderBy(s => s.Key).Select(s => s.Value).ToList();

        Assert.Equal("Imported Deck", actual.Title);
        Assert.Equal("Presenter Name", actual.Presenter);
        Assert.Equal(["Welcome", "Agenda", "Wrap Up"], slides.Select(s => s.Title));
        Assert.Equal("Say hello", slides[0].Notes);
        Assert.Equal(LiquidVictor.Enumerations.Layout.ImageRight, slides[1].Layout);

        var image = slides[1].ContentItems.Select(c => c.Value).Single(c => c.ContentType == "image/png");
        Assert.Equal(_pngBytes, image.Content);

        var contentItems = slides.SelectMany(s => s.ContentItems).Select(c => c.Value).ToList();
        Assert.Equal(4, contentItems.Count);
        Assert.All(contentItems, c => Assert.Contains("pptx:Source Deck.pptx", c.Tags));
    }

    private static ImportCommand CreateTarget(out ListLogger<ImportCommand> logger)
    {
        logger = new ListLogger<ImportCommand>();
        return new ImportCommand(new SlideDeckImporter(new ListLogger<SlideDeckImporter>()), logger);
    }

    private sealed class FakeWriteRepository : ISlideDeckWriteRepository
    {
        public List<SlideDeck> SavedSlideDecks { get; } = [];

        public void SaveSlideDeck(SlideDeck slideDeck) => this.SavedSlideDecks.Add(slideDeck);

        public void SaveSlide(Slide slide) => throw new NotSupportedException();

        public void SaveContentItem(ContentItem contentItem) => throw new NotSupportedException();
    }
}
