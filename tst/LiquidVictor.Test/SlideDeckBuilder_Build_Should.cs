using LiquidVictor.Builders;
using LiquidVictor.Data.YamlFile;
using Microsoft.Extensions.Configuration;

namespace LiquidVictor.Test;

public class SlideDeckBuilder_Build_Should
{
    private readonly string _lvDataPath;
    private readonly string _lvTempPath;

    private readonly SlideDeckReadRepository _readRepo;
    private readonly SlideDeckWriteRepository _writeRepo;

    public SlideDeckBuilder_Build_Should()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<SlideDeckBuilder_Build_Should>()
            .Build();

        _lvDataPath = config["LVDataPath"];
        _lvTempPath = Path.Combine(Path.GetTempPath(), "LiquidVictor");

        _readRepo = new SlideDeckReadRepository(_lvDataPath); // Read from the actual data store
        _writeRepo = new SlideDeckWriteRepository(_lvTempPath); // Write to a temp location
    }

    [Fact]
    public void ReturnAValidSlideDeck()
    {
        string titleContent = $"# {string.Empty.GetRandom()}";

        var slide = new SlideBuilder()
            .Id(Guid.NewGuid())
            .Title("Title Slide")
            .Layout("ImageRight")
            .TransitionIn("Slide")
            .TransitionOut("PresentationDefault")
            .ContentItems(new ContentItemsBuilder()
                .Add(new ContentItemBuilder()
                    .Id(Guid.NewGuid())
                    .Title("Title Content")
                    .ContentType("text/markdown")
                    .Content(titleContent)))
            .Build();

        var childContentItem = new ContentItemBuilder()
            .Id(Guid.NewGuid())
            .Title(string.Empty.GetRandom())
            .ContentType("text/markdown")
            .Content($"# {string.Empty.GetRandom()}")
            .Build();

        var slideDeck = new SlideDeckBuilder()
            .Id(Guid.NewGuid())
            .Title(string.Empty.GetRandom())
            .SubTitle(string.Empty.GetRandom())
            .Presenter("Liquid Victor")
            .ThemeName("moon")
            .AspectRatio("Widescreen")
            .Transition("Slide")
            .SlideDeckUrl($"https://example.com/{string.Empty.GetRandom()}")
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder(slide))
                .Add(new SlideBuilder()
                    .Layout("ImageRight")
                    .TransitionIn("PresentationDefault")
                    .TransitionOut("PresentationDefault")
                    .ContentItems(new ContentItemsBuilder()
                        .Add(new ContentItemBuilder()
                            .Id(Guid.NewGuid())
                            .Title("Title Slide")
                            .ContentType("text/markdown")
                            .Content(titleContent))
                        .Add(new ContentItemBuilder(childContentItem)))
            ))
            .Build();

        Assert.NotNull(slideDeck);
    }

    [Fact]
    public void ReturnAValidSlideDeckTheIncludesExistingSlides()
    {
        var titleContentTemplate = "## {PresentationTitle}\r\n\r\n#### {PresentationSubtitle}\r\n\r\n***\r\n\r\n### Barry S. Stahl\r\n\r\n### Solution Architect & Developer\r\n\r\n### [@bsstahl@cognitiveinheritance.com](https://fosstodon.org/@Bsstahl)\r\n\r\n### [https://CognitiveInheritance.com](https://cognitiveinheritance.com)\r\n";

        var presentationTitle = string.Empty.GetRandom();
        var presentationSubtitle = string.Empty.GetRandom();
        var titleContent = titleContentTemplate
            .Replace("{PresentationTitle}", presentationTitle)
            .Replace("{PresentationSubtitle}", presentationSubtitle);

        var slideDeckUrl = $"https://{presentationTitle}.azurewebsites.net";

        var favoritesSlide = _readRepo.GetSlide(Guid.Parse("636059f9-aa9d-4444-b4ec-dd7f62badd98"));
        var ossSlide = _readRepo.GetSlide(Guid.Parse("23528a73-7bc1-4e39-9f2f-c8b9e2cff982"));
        var foundationSlide = _readRepo.GetSlide(Guid.Parse("3129a405-82a7-432c-ae55-6f9d2335ab17"));
        var achievementSlide = _readRepo.GetSlide(Guid.Parse("39c6410c-3913-410b-abab-984014a15d84"));

        var backgroundContentItem = _readRepo.GetContentItem(Guid.Parse("19ae31f8-078b-4bd3-8411-0222b6a09c25"));
        var spacerContentItem = _readRepo.GetContentItem(Guid.Parse("685060d8-4205-4db9-b44d-9610a7729e7d"));

        var slideDeck = new SlideDeckBuilder()
            .Title(presentationTitle)
            .SubTitle(presentationSubtitle)
            .Presenter("Liquid Victor")
            .ThemeName("moon")
            .AspectRatio("Widescreen")
            .Transition("Slide")
            .SlideDeckUrl(slideDeckUrl)
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder()
                    .Layout("ImageRight")
                    .TransitionIn("PresentationDefault")
                    .TransitionOut("PresentationDefault")
                    .BackgroundContent(backgroundContentItem)
                    .ContentItems(new ContentItemsBuilder()
                        .Add(new ContentItemBuilder()
                            .Title("Title Slide")
                            .ContentType("text/markdown")
                            .Content(titleContent))
                        .Add(new ContentItemBuilder(spacerContentItem))))
                .Add(new SlideBuilder(favoritesSlide))
                .Add(new SlideBuilder(ossSlide))
                .Add(new SlideBuilder(foundationSlide))
                .Add(new SlideBuilder(achievementSlide)))
            .Build();

        Assert.NotNull(slideDeck);

        _writeRepo.SaveSlideDeck(slideDeck);
    }

    [Fact]
    public void SuccessfullySaveANewSlide()
    {
        var contentItemTitle = string.Empty.GetRandom();

        var contentItemBuilder = new ContentItemBuilder()
            .FileName($"http://example.com/{string.Empty.GetRandom()}")
            .Title(contentItemTitle)
            .ContentType("text/markdown")
            .Content($"# {string.Empty.GetRandom()}");

        var slideTitle = $"# {contentItemTitle}";
        string notes = string.Empty.GetRandom();

        var slideLayout = Enum.GetNames<Enumerations.Layout>().GetRandom();

        var slide = new SlideBuilder()
            .Title(slideTitle)
            .Layout(slideLayout)
            .Notes(notes)
            .ContentItems(contentItemBuilder)
            .Build();

        _writeRepo.SaveSlide(slide);
    }
}