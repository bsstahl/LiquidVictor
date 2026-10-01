using System.Text;
using LiquidVictor.Data.Hardcoded;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Business.Test;

public class SlideDeckRepository_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnConfiguredDataThroughEveryReadOperation()
    {
        var deckId = Guid.NewGuid();
        var slideId = Guid.NewGuid();
        var contentItemId = Guid.NewGuid();
        var backgroundContentItemId = Guid.NewGuid();
        var includeBlockId = Guid.NewGuid();

        var contentItem = new ContentItem(
            contentItemId,
            Encoding.UTF8.GetBytes("slide content"),
            "text/markdown",
            "slide.md",
            "Slide Content",
            "center",
            ["unit", "slide"]);
        var backgroundContentItem = new ContentItem(
            backgroundContentItemId,
            [1, 2, 3],
            "image/png",
            "background.png",
            "Background",
            "cover",
            ["unit", "background"]);
        var slide = new Slide(
            slideId,
            "Configured Slide",
            Layout.FullPage,
            Transition.Fade,
            Transition.Slide,
            "Presenter notes",
            backgroundContentItem,
            true,
            [new KeyValuePair<int, ContentItem>(10, contentItem)])
        {
            BackgroundTransitionIn = Transition.Fancy,
            BackgroundTransitionOut = Transition.None
        };
        var includeBlock = new IncludeBlock(new[] { slide }.OrderBy(_ => 0));
        var deck = new SlideDeck(
            deckId,
            "Configured Deck",
            "Subtitle",
            "Presenter",
            "moon",
            new Uri("https://example.com/configured"),
            "Print",
            Transition.Fancy,
            AspectRatio.Standard,
            Format.Workshop,
            new[] { includeBlock }.OrderBy(_ => 0))
        {
            BackgroundTransition = Transition.Fade,
            BackgroundContent = backgroundContentItem
        };
        var repository = new SlideDeckRepository(
            [deck],
            [slide],
            [contentItem, backgroundContentItem],
            [new KeyValuePair<Guid, IncludeBlock>(includeBlockId, includeBlock)]);

        Assert.Same(deck, repository.GetSlideDeck(deckId));
        Assert.Same(slide, repository.GetSlide(slideId));
        Assert.Same(contentItem, repository.GetContentItem(contentItemId));
        Assert.Same(includeBlock, repository.GetIncludeBlock(includeBlockId));
        Assert.Equal([deckId], repository.GetSlideDeckIds());
        Assert.Equal([slideId], repository.GetSlideIds());
        Assert.Equal([contentItemId, backgroundContentItemId], repository.GetContentItemIds());
        Assert.Equal([includeBlockId], repository.GetIncludeBlockIds());
        Assert.Same(deck, Assert.Single(repository.GetSlideDecks()));
        Assert.Same(slide, Assert.Single(repository.GetSlides()));
        Assert.Equal([contentItem, backgroundContentItem], repository.GetContentItems());
        Assert.Same(includeBlock, Assert.Single(repository.GetIncludeBlocks()));

        Assert.Equal(Format.Workshop, deck.Format);
        Assert.Equal(AspectRatio.Standard, deck.AspectRatio);
        Assert.Equal(new Uri("https://example.com/configured"), deck.SlideDeckUrl);
        Assert.Equal(Transition.Fade, deck.BackgroundTransition);
        Assert.Equal(Transition.Fade, slide.TransitionIn);
        Assert.Equal(Transition.Slide, slide.TransitionOut);
        Assert.Equal(Transition.Fancy, slide.BackgroundTransitionIn);
        Assert.Equal(Transition.None, slide.BackgroundTransitionOut);
        Assert.Equal("Presenter notes", slide.Notes);
        Assert.True(slide.NeverFullScreen);
        Assert.Equal("center", contentItem.Alignment);
        Assert.Equal(["unit", "slide"], contentItem.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void IncludeTheCurrentEntityDataInTheDemoFixture()
    {
        var repository = new SlideDeckRepository();
        var deck = repository.GetSlideDeck(Guid.Parse("E0B187D2-C9B7-4635-8FE5-0CA21BC5007F"));
        var slide = repository.GetSlide(Guid.Parse("20000000-0000-0000-0000-000000000001"));
        var contentItem = repository.GetContentItem(Guid.Parse("10000000-0000-0000-0000-000000000002"));

        Assert.Equal(3, repository.GetSlideDecks().Count());
        Assert.Equal(4, repository.GetSlides().Count());
        Assert.Equal(7, repository.GetContentItems().Count());
        Assert.Equal(9, repository.GetIncludeBlocks().Count());
        Assert.Equal("black", deck.ThemeName);
        Assert.Equal(new Uri("https://example.com/demo"), deck.SlideDeckUrl);
        Assert.Equal(Format.Session, deck.Format);
        Assert.Equal(AspectRatio.Widescreen, deck.AspectRatio);
        Assert.Equal(Transition.Slide, deck.Transition);
        Assert.Equal(Transition.Fade, deck.BackgroundTransition);
        Assert.NotNull(deck.BackgroundContent);
        Assert.Equal(Transition.PresentationDefault, slide.TransitionIn);
        Assert.Equal(Transition.PresentationDefault, slide.TransitionOut);
        Assert.Equal(Transition.Fade, slide.BackgroundTransitionIn);
        Assert.Equal(Transition.Slide, slide.BackgroundTransitionOut);
        Assert.Equal("Notes for Full Screen Slide", slide.Notes);
        Assert.Same(deck.BackgroundContent, slide.BackgroundContent);
        Assert.False(slide.NeverFullScreen);
        Assert.Equal("bullet-points.md", contentItem.FileName);
        Assert.Equal("Bullet Points", contentItem.Title);
        Assert.Equal("left", contentItem.Alignment);
        Assert.Equal(["demo", "bullets"], contentItem.Tags);
        Assert.Equal("* Bullet Point 1\r\n* Bullet Point 2", Encoding.UTF8.GetString(contentItem.Content));
    }
}
