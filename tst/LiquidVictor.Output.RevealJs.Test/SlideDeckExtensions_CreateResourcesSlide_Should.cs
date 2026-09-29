using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using LiquidVictor.Output.RevealJs.Extensions;

namespace LiquidVictor.Output.RevealJs.Test;

public class SlideDeckExtensions_CreateResourcesSlide_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void CreateAFlatListWhenAllResourcesHaveTheDefaultType()
    {
        var slideDeck = new SlideDeck();
        slideDeck.Resources.Add(new Resource { Name = "First", Url = "https://example.com/1" });
        slideDeck.Resources.Add(new Resource { Name = "Second", Url = "https://example.com/2" });

        var slide = slideDeck.CreateResourcesSlide();

        Assert.Equal("Resources", slide.Title);
        Assert.Equal(LiquidVictor.Enumerations.Layout.ImageRight, slide.Layout);
        Assert.Equal(
            "- <a href=\"https://example.com/1\">First</a>" + Environment.NewLine +
            "- <a href=\"https://example.com/2\">Second</a>" + Environment.NewLine,
            Encoding.UTF8.GetString(slide.ContentItems.Single().Value.Content));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GroupResourcesInFirstSeenOrderAndKeepResourceOrder()
    {
        var slideDeck = new SlideDeck();
        slideDeck.Resources.Add(new Resource { Name = "Video 1", Type = "Videos", Url = "https://example.com/1" });
        slideDeck.Resources.Add(new Resource { Name = "Other", Url = "https://example.com/2" });
        slideDeck.Resources.Add(new Resource { Name = "Video 2", Type = "Videos", Url = "https://example.com/3" });

        var slide = slideDeck.CreateResourcesSlide();

        Assert.Equal(
            "**Videos**" + Environment.NewLine +
            "- <a href=\"https://example.com/1\">Video 1</a>" + Environment.NewLine +
            "- <a href=\"https://example.com/3\">Video 2</a>" + Environment.NewLine +
            Environment.NewLine +
            "**Other**" + Environment.NewLine +
            "- <a href=\"https://example.com/2\">Other</a>" + Environment.NewLine,
            Encoding.UTF8.GetString(slide.ContentItems.First().Value.Content));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void OmitImageWhenDeckUrlIsMissing()
    {
        var slideDeck = new SlideDeck();
        slideDeck.Resources.Add(new Resource { Name = "First", Url = "https://example.com/1" });

        var slide = slideDeck.CreateResourcesSlide();

        Assert.Single(slide.ContentItems);
        Assert.All(slide.ContentItems, contentItem => Assert.StartsWith("text/", contentItem.Value.ContentType));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CreatePngQrCodeWhenDeckUrlIsPresent()
    {
        var slideDeck = new SlideDeck { SlideDeckUrl = new Uri("https://example.com/deck") };
        slideDeck.Resources.Add(new Resource { Name = "First", Url = "https://example.com/1" });

        var slide = slideDeck.CreateResourcesSlide();
        var image = Assert.Single(slide.ContentItems, item => item.Value.ContentType == "image/png").Value;

        Assert.Equal("resources-qr-code.png", image.FileName);
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4e, 0x47 }, image.Content.Take(4));
    }
}
