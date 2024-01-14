using LiquidVictor.Builders;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using TestHelperExtensions;

namespace LiquidVictor.Data.Test.Extensions;

public static class SlideBuilderExtensions
{
    public static SlideBuilder UseRandomValues(this SlideBuilder builder)
    {
        return builder
            .BackgroundContent(new ContentItemBuilder().UseRandomValues())
            .ContentItems(new ContentItemBuilder().UseRandomValues())
            .Id(Guid.NewGuid())
            .Layout(Layout.FullPage.GetRandom())
            .NeverFullScreen(true.GetRandom())
            .Notes(string.Empty.GetRandom())
            .Title(string.Empty.GetRandom())
            .TransitionIn(Transition.Fancy.GetRandom())
            .TransitionOut(Transition.Fancy.GetRandom());
    }
}
