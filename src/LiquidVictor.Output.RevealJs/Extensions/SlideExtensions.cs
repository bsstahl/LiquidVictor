using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Output.RevealJs.Entities;
using LiquidVictor.Output.RevealJs.Interfaces;

namespace LiquidVictor.Output.RevealJs.Extensions;

public static class SlideExtensions
{
    public static string GetLayout(this Slide slide, int zeroBasedSlideIndex, ILayoutStrategy[] layoutStrategies)
    {
        ArgumentNullException.ThrowIfNull(slide);

        var strategy = layoutStrategies?[(int)slide.Layout];
        return strategy is null 
            ? throw new NotSupportedException($"No layout strategy found for {slide.Layout}") 
            : strategy.Layout(slide, zeroBasedSlideIndex);
    }

    public static bool MakeSoloImageFullScreen(this Slide slide, BuilderOptions builderOptions)
    {
        ArgumentNullException.ThrowIfNull(slide);
        ArgumentNullException.ThrowIfNull(builderOptions);

        return builderOptions.MakeSoloImagesFullScreen &&
             string.IsNullOrWhiteSpace(slide.Title) &&
             !slide.NeverFullScreen &&
             slide.ContentItems.Count == 1 &&
             slide.ContentItems.First().Value.IsImage();
    }

    public static string AsStartSlideSection(this Slide slide, Transition presentationDefaultTransition, Transition presentationDefaultBackgroundTransition, ContentItem? presentationDefaultBackgroundContent = null)
    {
        ArgumentNullException.ThrowIfNull(slide);

        string transitionClass = $"{slide.TransitionIn.GetClass(true, presentationDefaultTransition)} {slide.TransitionOut.GetClass(false, presentationDefaultTransition)}".Trim();
        string backgroundTransitionClass = $"{slide.BackgroundTransitionIn.GetClass(true, presentationDefaultBackgroundTransition)} {slide.BackgroundTransitionOut.GetClass(false, presentationDefaultBackgroundTransition)}".Trim();

        string result = "<section";
        if (!string.IsNullOrWhiteSpace(transitionClass))
            result += $" data-transition=\"{transitionClass}\"";
        if (!string.IsNullOrWhiteSpace(backgroundTransitionClass))
            result += $" data-background-transition=\"{backgroundTransitionClass}\"";

        var backgroundContent = slide.BackgroundContent ?? presentationDefaultBackgroundContent;
        if (backgroundContent != null)
        {
            string backgroundId = backgroundContent.Id.ToString();
            string backgroundExtension = System.IO.Path.GetExtension(backgroundContent.FileName);
            result += $" data-background=\'img/{backgroundId}{backgroundExtension}\'";
        }

        result += ">";
        return result;
    }

}
