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
}
