using LiquidVictor.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using LiquidVictor.Enumerations;
using System.Diagnostics.CodeAnalysis;

namespace LiquidVictor.Business.Test;

[ExcludeFromCodeCoverage]
public class SlideDeck_Slides_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnTheCorrectSlidesFromSingleSlideOnlyIncludes()
    {
        var services = new ServiceCollection()
            .AddTransient<ISlideDeckReadRepository>(c => new Data.Hardcoded.SlideDeckRepository())
            .BuildServiceProvider();

        var slideDeckId = Guid.Parse("E0B187D2-C9B7-4635-8FE5-0CA21BC5007F");
        var readRepo = services.GetRequiredService<ISlideDeckReadRepository>();

        var slideDeck = readRepo.GetSlideDeck(slideDeckId);
        var slides = slideDeck.Slides;

        Assert.Equal(slideDeckId, slideDeck.Id);
        Assert.Equal(4, slides.Count());

        var firstSlide = slides.FirstOrDefault().Value;
        Assert.NotNull(firstSlide);
        Assert.Equal("Full Screen Slide", firstSlide.Title);
        Assert.Equal(Layout.FullPage, firstSlide.Layout);
        Assert.Equal("text/markdown", firstSlide.ContentItems.Single().Value.ContentType);

        var secondSlide = slides.Skip(1).FirstOrDefault().Value;
        Assert.NotNull(secondSlide);
        Assert.Equal("Paragraph Slide", secondSlide.Title);
        Assert.Equal(Layout.FullPageFragments, secondSlide.Layout);
        Assert.Equal("text/plain", secondSlide.ContentItems.Single().Value.ContentType);

        var thirdSlide = slides.Skip(2).FirstOrDefault().Value;
        var thirdSlideFirstContentItem = thirdSlide.ContentItems.First().Value;
        var thirdSlideSecondContentItem = thirdSlide.ContentItems.Skip(1).First().Value;
        Assert.NotNull(thirdSlide);
        Assert.Equal("Image-Right Slide", thirdSlide.Title);
        Assert.Equal(Layout.ImageRight, thirdSlide.Layout);
        Assert.Equal("text/markdown", thirdSlideFirstContentItem.ContentType);
        Assert.Equal("image/svg", thirdSlideSecondContentItem.ContentType);

        var fourthSlide = slides.Last().Value;
        var fourthSlideFirstContentItem = fourthSlide.ContentItems.First().Value;
        var fourthSlideSecondContentItem = fourthSlide.ContentItems.Skip(1).First().Value;
        Assert.NotNull(thirdSlide);
        Assert.Equal("Image-Left Slide", fourthSlide.Title);
        Assert.Equal(Layout.ImageLeft, fourthSlide.Layout);
        Assert.Equal("text/markdown", fourthSlideFirstContentItem.ContentType);
        Assert.Equal("image/png", fourthSlideSecondContentItem.ContentType);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnTheCorrectSlidesFromASimpleGroupedDeck()
    {
        var services = new ServiceCollection()
            .AddTransient<ISlideDeckReadRepository>(c => new Data.Hardcoded.SlideDeckRepository())
            .BuildServiceProvider();

        var slideDeckId = Guid.Parse("728f2f58-9ee6-4e5a-9281-26d094b7a68a");
        var readRepo = services.GetRequiredService<ISlideDeckReadRepository>();

        var slideDeck = readRepo.GetSlideDeck(slideDeckId);
        var slides = slideDeck.Slides;

        Assert.Equal(slideDeckId, slideDeck.Id);
        Assert.Equal(4, slides.Count());

        var firstSlide = slides.FirstOrDefault().Value;
        Assert.NotNull(firstSlide);
        Assert.Equal("Paragraph Slide", firstSlide.Title);
        Assert.Equal(Layout.FullPageFragments, firstSlide.Layout);
        Assert.Equal("text/plain", firstSlide.ContentItems.Single().Value.ContentType);

        var secondSlide = slides.Skip(1).FirstOrDefault().Value;
        Assert.NotNull(secondSlide);
        Assert.Equal("Full Screen Slide", secondSlide.Title);
        Assert.Equal(Layout.FullPage, secondSlide.Layout);
        Assert.Equal("text/markdown", secondSlide.ContentItems.Single().Value.ContentType);

        var thirdSlide = slides.Skip(2).FirstOrDefault().Value;
        var thirdSlideFirstContentItem = thirdSlide.ContentItems.First().Value;
        var thirdSlideSecondContentItem = thirdSlide.ContentItems.Skip(1).First().Value;
        Assert.NotNull(thirdSlide);
        Assert.Equal("Image-Left Slide", thirdSlide.Title);
        Assert.Equal(Layout.ImageLeft, thirdSlide.Layout);
        Assert.Equal("text/markdown", thirdSlideFirstContentItem.ContentType);
        Assert.Equal("image/png", thirdSlideSecondContentItem.ContentType);

        var fourthSlide = slides.Last().Value;
        var fourthSlideFirstContentItem = fourthSlide.ContentItems.First().Value;
        var fourthSlideSecondContentItem = fourthSlide.ContentItems.Skip(1).First().Value;
        Assert.NotNull(fourthSlide);
        Assert.Equal("Image-Right Slide", fourthSlide.Title);
        Assert.Equal(Layout.ImageRight, fourthSlide.Layout);
        Assert.Equal("text/markdown", fourthSlideFirstContentItem.ContentType);
        Assert.Equal("image/svg", fourthSlideSecondContentItem.ContentType);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnTheCorrectSlidesFromAMixedDeck()
    {
        var services = new ServiceCollection()
            .AddTransient<ISlideDeckReadRepository>(c => new Data.Hardcoded.SlideDeckRepository())
            .BuildServiceProvider();

        var slideDeckId = Guid.Parse("bcf43e23-4771-4862-baf7-9bccb9c096c5");
        var readRepo = services.GetRequiredService<ISlideDeckReadRepository>();

        var slideDeck = readRepo.GetSlideDeck(slideDeckId);
        var slides = slideDeck.Slides;

        Assert.Equal(slideDeckId, slideDeck.Id);
        Assert.Equal(4, slides.Count());

        var firstSlide = slides.FirstOrDefault().Value;
        var firstSlideFirstContentItem = firstSlide.ContentItems.First().Value;
        var firstSlideSecondContentItem = firstSlide.ContentItems.Skip(1).First().Value;
        Assert.NotNull(firstSlide);
        Assert.Equal("Image-Right Slide", firstSlide.Title);
        Assert.Equal(Layout.ImageRight, firstSlide.Layout);
        Assert.Equal("text/markdown", firstSlideFirstContentItem.ContentType);
        Assert.Equal("image/svg", firstSlideSecondContentItem.ContentType);

        var secondSlide = slides.Skip(1).FirstOrDefault().Value;
        Assert.NotNull(secondSlide);
        Assert.Equal("Full Screen Slide", secondSlide.Title);
        Assert.Equal(Layout.FullPage, secondSlide.Layout);
        Assert.Equal("text/markdown", secondSlide.ContentItems.Single().Value.ContentType);

        var thirdSlide = slides.Skip(2).FirstOrDefault().Value;
        var thirdSlideFirstContentItem = thirdSlide.ContentItems.First().Value;
        var thirdSlideSecondContentItem = thirdSlide.ContentItems.Skip(1).First().Value;
        Assert.NotNull(thirdSlide);
        Assert.Equal("Image-Left Slide", thirdSlide.Title);
        Assert.Equal(Layout.ImageLeft, thirdSlide.Layout);
        Assert.Equal("text/markdown", thirdSlideFirstContentItem.ContentType);
        Assert.Equal("image/png", thirdSlideSecondContentItem.ContentType);

        var fourthSlide = slides.Last().Value;
        Assert.NotNull(fourthSlide);
        Assert.Equal("Paragraph Slide", fourthSlide.Title);
        Assert.Equal(Layout.FullPageFragments, fourthSlide.Layout);
        Assert.Equal("text/plain", fourthSlide.ContentItems.Single().Value.ContentType);

    }
}