using LiquidVictor.Enumerations;
using TestHelperExtensions;
using LiquidVictor.Builders;

namespace LiquidVictor.Data.YamlFile.Test;

public class SlideDeckWriteRepository_SaveSlideDeck_Should
{
    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Status", "Incomplete")]
    public void WriteTheSampleSlideDeck()
    {
        // Create Slide Deck
        var slideDeck = new Builders.SlideDeckBuilder()
            .Id(Guid.NewGuid())
            .Title($"Presentation_{string.Empty.GetRandom()}")
            .SubTitle(string.Empty.GetRandom())
            .Presenter(string.Empty.GetRandom())
            .ThemeName(string.Empty.GetRandom())
            .PrintLinkText($"{string.Empty.GetRandom()} {string.Empty.GetRandom()}")
            .AspectRatio(Enum.GetNames<AspectRatio>().GetRandom())
            .Transition(Enum.GetNames<Transition>().GetRandom())
            .SlideDeckUrl($"https://example.com/{string.Empty.GetRandom()}")
            .Slides(new Builders.SlidesBuilder()
                .Add(new Builders.SlideBuilder()
                    .Id(Guid.NewGuid())
                    .Title(string.Empty.GetRandom())
                    .Layout(Enum.GetNames<Layout>().GetRandom())
                    .TransitionIn(Enum.GetNames<Transition>().GetRandom())
                    .TransitionOut(Enum.GetNames<Transition>().GetRandom())
                    .Notes(string.Empty.GetRandom())
                    .ContentItems(new Builders.ContentItemsBuilder()
                        .Add(new ContentItemBuilder()
                            .Id(Guid.NewGuid())
                            .ContentType("text/markdown")
                            .Title(string.Empty.GetRandom())
                            .Content(string.Empty.GetRandom())))))
            .Build();

        var repoPath = Path.GetFullPath(@".\TestRepo");
        var repo = new SlideDeckWriteRepository(repoPath);
        repo.SaveSlideDeck(slideDeck);

        throw new NotImplementedException();
    }
}