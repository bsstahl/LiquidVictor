using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Output.RevealJs.Entities;
using LiquidVictor.Output.RevealJs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class SlideExtensions
    {
        public static string GetLayout(this Slide slide, ILayoutStrategy[] layoutStrategies)
        {
            var strategy = layoutStrategies[(int)slide.Layout];
            return strategy == null 
                ? throw new NotSupportedException($"No layout strategy found for {slide.Layout}") 
                : strategy.Layout(slide);
        }

        public static bool MakeSoloImageFullScreen(this Slide slide, BuilderOptions builderOptions)
        {
            return builderOptions.MakeSoloImagesFullScreen &&
                 string.IsNullOrWhiteSpace(slide.Title) &&
                 !slide.NeverFullScreen &&
                 slide.ContentItems.Count() == 1 &&
                 slide.ContentItems.First().Value.IsImage();
        }

        public static string AsStartSlideSection(this Slide slide, Transition presentationDefaultTransition)
        {
            string transitionClass = $"{slide.TransitionIn.GetClass(true, presentationDefaultTransition)} {slide.TransitionOut.GetClass(false, presentationDefaultTransition)}".Trim();

            string result = "<section";
            if (!string.IsNullOrWhiteSpace(transitionClass))
                result += $" data-transition=\"{transitionClass}\"";

            if (slide.BackgroundContent != null)
            {
                var bgContent = slide.BackgroundContent;
                string backgroundId = bgContent.Id.ToString();
                string backgroundExtension = System.IO.Path.GetExtension(bgContent.FileName);
                result += $" data-background=\'img/{backgroundId}{backgroundExtension}\'";
            }

            result += ">";
            return result;
        }

        public static bool IsText(this ContentItem contentItem)
        {
            return contentItem.ContentType.ToLower().StartsWith("text");
        }

        public static IEnumerable<KeyValuePair<int, ContentItem>> TextContentItems(this IEnumerable<KeyValuePair<int, ContentItem>> contentItems)
        {
            return contentItems.Where(c => c.Value.IsText());
        }

        public static IEnumerable<KeyValuePair<int, ContentItem>> ImageContentItems(this IEnumerable<KeyValuePair<int, ContentItem>> contentItems)
        {
            return contentItems.Where(c => c.Value.IsImage());
        }

        public static bool IsImage(this ContentItem contentItem)
        {
            return contentItem.ContentType.ToLower().StartsWith("image");
        }

    }
}
