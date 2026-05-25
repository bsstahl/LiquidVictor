using LiquidVictor.Builders;
using LiquidVictor.Entities;

namespace LiquidVictor.Output.Jupyter.Extensions;

public static class SlideDeckExtensions
{
    public static Slide CreateTitleSlide(this SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNull(slideDeck);

        return new SlideBuilder()
            .Title(slideDeck.Title)
            .ContentItems(new ContentItemBuilder()
                .ContentType("text/markdown")
                .Content($"## {slideDeck.SubTitle}"))
            .ContentItems(new ContentItemBuilder()
                .ContentType("text/markdown")
                .Content($"**{slideDeck.Presenter}**"))
            .Build();
    }
}
