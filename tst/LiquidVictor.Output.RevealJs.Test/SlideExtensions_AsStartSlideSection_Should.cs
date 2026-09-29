using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Output.RevealJs.Extensions;

namespace LiquidVictor.Output.RevealJs.Test;

public class SlideExtensions_AsStartSlideSection_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void IncludeBackgroundTransitionAttributeWhenSlideTransitionsAreSpecified()
    {
        var slide = new Slide
        {
            TransitionIn = Transition.Slide,
            TransitionOut = Transition.Fade,
            BackgroundTransitionIn = Transition.Fade,
            BackgroundTransitionOut = Transition.Slide
        };

        var result = slide.AsStartSlideSection(Transition.Slide, Transition.Fade);

        Assert.Contains("data-transition=\"slide-in fade-out\"", result);
        Assert.Contains("data-background-transition=\"fade-in slide-out\"", result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void UsePresentationBackgroundTransitionWhenSlideUsesPresentationDefault()
    {
        var slide = new Slide
        {
            TransitionIn = Transition.PresentationDefault,
            TransitionOut = Transition.PresentationDefault,
            BackgroundTransitionIn = Transition.PresentationDefault,
            BackgroundTransitionOut = Transition.PresentationDefault
        };

        var result = slide.AsStartSlideSection(Transition.Slide, Transition.Fade);

        Assert.Contains("data-transition=\"slide-in slide-out\"", result);
        Assert.Contains("data-background-transition=\"fade-in fade-out\"", result);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(FooterDisplay.Default, false, null, true)]
    [InlineData(FooterDisplay.Default, true, null, false)]
    [InlineData(FooterDisplay.Default, false, false, false)]
    [InlineData(FooterDisplay.Default, true, true, true)]
    [InlineData(FooterDisplay.Always, true, null, true)]
    [InlineData(FooterDisplay.Always, false, false, true)]
    [InlineData(FooterDisplay.Never, false, null, false)]
    [InlineData(FooterDisplay.Never, true, true, false)]
    public void HideFooterOnlyWhenFooterShouldNotBeShown(FooterDisplay presentationFooterDisplay, bool hasBackgroundContent, bool? showFooter, bool expectFooter)
    {
        var slide = new Slide
        {
            BackgroundContent = hasBackgroundContent
                ? new ContentItem { Id = Guid.NewGuid(), FileName = "background.png", ContentType = "image/png" }
                : null,
            ShowFooter = showFooter
        };

        var result = slide.AsStartSlideSection(Transition.Slide, Transition.Fade, null, presentationFooterDisplay);

        Assert.Equal(expectFooter, slide.ShowFooter(presentationFooterDisplay));
        if (expectFooter)
            Assert.DoesNotContain("data-state=\"hide-footer\"", result);
        else
            Assert.Contains("data-state=\"hide-footer\"", result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ShowFooterWhenOnlyThePresentationDefaultBackgroundIsUsed()
    {
        var slide = new Slide();
        var deckBackground = new ContentItem { Id = Guid.NewGuid(), FileName = "deck.png", ContentType = "image/png" };

        var result = slide.AsStartSlideSection(Transition.Slide, Transition.Fade, deckBackground);

        Assert.Contains("data-background=", result);
        Assert.DoesNotContain("data-state=\"hide-footer\"", result);
    }
}
