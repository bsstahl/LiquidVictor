using LiquidVictor.Builders;
using LiquidVictor.Data.YamlFile;
using Microsoft.Extensions.Configuration;

namespace LiquidVictor.Test;

public class SlideDeckBuilder_Build_Should
{
    [Fact]
    public void ReturnAValidSlideDeck()
    {
        string titleContent = $"# {string.Empty.GetRandom()}";

        var slide = new SlideBuilder()
            .Title(string.Empty.GetRandom())
            .Layout("ImageRight")
            .TransitionIn("PresentationDefault")
            .TransitionOut("PresentationDefault")
            .ContentItems(new ContentItemsBuilder()
                .Add(new ContentItemBuilder()
                    .Id(Guid.NewGuid())
                    .Title("Title Slide")
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
    public void AnotherThing()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<SlideDeckBuilder_Build_Should>()
            .Build();

        var lvDataPath = config["LVDataPath"];

        var buildScriptTemplate = "..\\..\\..\\LiquidVictor\\src\\LV\\bin\\Release\\net6.0\\publish\\LV.exe build -OutputEngineType:RevealJS -SourceRepoType:jsonFileSystem -SourceRepoPath:..\\..\\..\\LiquidVictorDatabases\\JsonFileSystem\\Bsstahl\\ -SlideDeckId:{SlideDeckId} -TemplatePath:..\\..\\..\\LiquidVictor\\Templates\\RevealJS \"-PresentationPath:..\\..\\..\\{PresentationDomain}Presentations\\Presentations\\{PresentationTitle}\\{PresentationTitle} - {PresentationType}\" --NoTitle";
        var titleContentTemplate = "## {PresentationTitle}\r\n\r\n#### {PresentationSubtitle}\r\n\r\n***\r\n\r\n### Barry S. Stahl\r\n\r\n### Solution Architect & Developer\r\n\r\n### [@bsstahl@cognitiveinheritance.com](https://fosstodon.org/@Bsstahl)\r\n\r\n### [https://CognitiveInheritance.com](https://cognitiveinheritance.com)\r\n";

        var presentationTitle = string.Empty.GetRandom();
        var presentationSubtitle = string.Empty.GetRandom();
        var titleContent = titleContentTemplate
            .Replace("{PresentationTitle}", presentationTitle)
            .Replace("{PresentationSubtitle}", presentationSubtitle);

        var slideDeckUrl = $"https://{presentationTitle}.azurewebsites.net";

        var readRepo = new SlideDeckReadRepository(lvDataPath);

        var favoritesSlide = readRepo.GetSlide(Guid.Parse("636059f9-aa9d-4444-b4ec-dd7f62badd98"));
        var ossSlide = readRepo.GetSlide(Guid.Parse("23528a73-7bc1-4e39-9f2f-c8b9e2cff982"));
        var givecampSlide = readRepo.GetSlide(Guid.Parse("3129a405-82a7-432c-ae55-6f9d2335ab17"));
        var achievementSlide = readRepo.GetSlide(Guid.Parse("39c6410c-3913-410b-abab-984014a15d84"));

        var backgroundContentItem = readRepo.GetContentItem(Guid.Parse("19ae31f8-078b-4bd3-8411-0222b6a09c25"));
        var spacerContentItem = readRepo.GetContentItem(Guid.Parse("685060d8-4205-4db9-b44d-9610a7729e7d"));

        var presentationId = Guid.NewGuid();
        var slideDeck = new SlideDeckBuilder()
            .Id(presentationId)
            .Title(presentationTitle)
            .SubTitle(presentationSubtitle)
            .Presenter("Liquid Victor")
            .ThemeName("moon")
            .AspectRatio("Widescreen")
            .Transition("Slide")
            .SlideDeckUrl(slideDeckUrl)
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder(favoritesSlide))
                .Add(new SlideBuilder(ossSlide))
                .Add(new SlideBuilder(givecampSlide))
                .Add(new SlideBuilder(achievementSlide))
                .Add(new SlideBuilder()
                    .Layout("ImageRight")
                    .TransitionIn("PresentationDefault")
                    .TransitionOut("PresentationDefault")
                    .BackgroundContent(backgroundContentItem)
                    .ContentItems(new ContentItemsBuilder()
                        .Add(new ContentItemBuilder()
                            .Id(Guid.NewGuid())
                            .Title("Title Slide")
                            .ContentType("text/markdown")
                            .Content(titleContent))
                        .Add(new ContentItemBuilder(spacerContentItem)))
            ))
            .Build();

        Assert.NotNull(slideDeck);
        //var writeRepo = new LiquidVictor.Data.YamlFile.SlideDeckWriteRepository(MyExtensions.LVDataPath);
        //writeRepo.SaveSlideDeck(slideDeck);

    }
}
