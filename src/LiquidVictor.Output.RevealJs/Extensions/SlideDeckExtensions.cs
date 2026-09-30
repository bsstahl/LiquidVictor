using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using LiquidVictor.Extensions;
using QRCoder;

namespace LiquidVictor.Output.RevealJs.Extensions;

public static class SlideDeckExtensions
{
    public static Slide CreateTitleSlide(this SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNull(slideDeck);

        var titleSlide = new Slide()
        {
            Id = Guid.NewGuid(),
            Title = slideDeck.Title,
            Layout = Enumerations.Layout.Title
        };

        titleSlide.ContentItems.Add(
            new KeyValuePair<int, ContentItem>(1,
            new ContentItem()
            {
                Content = slideDeck.SubTitle.AsByteArray(),
                ContentType = "text/plain",
                Id = Guid.NewGuid()
            }));

        titleSlide.ContentItems.Add(
            new KeyValuePair<int, ContentItem>(2,
            new ContentItem()
            {
                Content = slideDeck.Presenter.AsByteArray(),
                ContentType = "text/plain",
                Id = Guid.NewGuid()
            }));

        string url = slideDeck.SlideDeckUrl?.ToString() ?? "about:blank";
        titleSlide.ContentItems.Add(
            new KeyValuePair<int, ContentItem>(3,
            new ContentItem()
            {
                Content = url.AsByteArray(),
                ContentType = "text/plain",
                Id = Guid.NewGuid()
            }));

        string linkText = slideDeck.PrintLinkText ?? " ";
        titleSlide.ContentItems.Add(
            new KeyValuePair<int, ContentItem>(3,
            new ContentItem()
            {
                Content = linkText.AsByteArray(),
                ContentType = "text/plain",
                Id = Guid.NewGuid()
            }));

        return titleSlide;
    }

    public static Slide CreateResourcesSlide(this SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNull(slideDeck);

        var resourcesSlide = new Slide()
        {
            Id = Guid.NewGuid(),
            Title = "Resources",
            Layout = Enumerations.Layout.ImageRight
        };

        bool hasTypedResources = slideDeck.Resources.Any(resource => resource.Type != "Other");
        var content = new StringBuilder();
        foreach (var group in slideDeck.Resources.GroupBy(resource => resource.Type))
        {
            if (hasTypedResources)
            {
                if (content.Length > 0)
                    content.AppendLine();
                content.Append("**").Append(System.Net.WebUtility.HtmlEncode(group.Key)).AppendLine("**");
            }

            foreach (var resource in group)
            {
                content.Append("- <a href=\"")
                    .Append(System.Net.WebUtility.HtmlEncode(resource.Url))
                    .Append("\">")
                    .Append(System.Net.WebUtility.HtmlEncode(resource.Name))
                    .AppendLine("</a>");
            }
        }

        resourcesSlide.ContentItems.Add(new KeyValuePair<int, ContentItem>(0, new ContentItem
        {
            Id = Guid.NewGuid(),
            Content = content.ToString().AsByteArray(),
            ContentType = "text/markdown"
        }));

        if (slideDeck.SlideDeckUrl is not null)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(slideDeck.SlideDeckUrl.ToString(), QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            resourcesSlide.ContentItems.Add(new KeyValuePair<int, ContentItem>(1, new ContentItem
            {
                Id = Guid.NewGuid(),
                Content = qrCode.GetGraphic(20),
                ContentType = "image/png",
                FileName = "resources-qr-code.png"
            }));
        }

        return resourcesSlide;
    }

    public static (int, int) GetPresentationSize(this SlideDeck deck)
    {
        ArgumentNullException.ThrowIfNull(deck);

        int width, height;
        switch (deck.AspectRatio)
        {
            case Enumerations.AspectRatio.Widescreen: // 16:9
                width = 1920;
                height = 1080;
                break;
            case Enumerations.AspectRatio.Standard: // 4:3
                width = 1024;
                height = 768;
                break;
            default:
                throw new NotSupportedException($"Invalid Aspect Ratio {deck.AspectRatio}");
        }

        return (width, height);
    }
}
