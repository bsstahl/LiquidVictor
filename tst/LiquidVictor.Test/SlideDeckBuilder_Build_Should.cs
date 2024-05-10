using LiquidVictor.Builders;

namespace LiquidVictor.Test;

public class SlideDeckBuilder_Build_Should
{
    [Fact]
    public void ReturnAValidSlideDeck()
    {
        string titleContent = $"# {string.Empty.GetRandom()}";

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
}
