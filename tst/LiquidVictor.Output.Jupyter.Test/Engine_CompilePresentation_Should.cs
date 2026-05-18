using LiquidVictor.Builders;
using LiquidVictor.Output.Jupyter.Entities;
using LiquidVictor.Output.Jupyter.Generator;

namespace LiquidVictor.Output.Jupyter.Test;

public class Engine_CompilePresentation_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowForUnsupportedContentType()
    {
        var engine = new Engine(new BuilderOptions());
        var slideDeck = new SlideDeckBuilder()
            .Title("Notebook Deck")
            .Slides(new SlidesBuilder()
                .Add(new SlideBuilder()
                    .Title("First Slide")
                    .ContentItems(new ContentItemBuilder()
                        .ContentType("application/octet-stream")
                        .Content([1, 2, 3]))))
            .Build();

        Assert.Throws<NotSupportedException>(() => engine.CompilePresentation(slideDeck));
    }
}
