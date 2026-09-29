using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs.Extensions;

public static class ContentItemExtensions
{
    public static string AsStartContentItemSlideSection(this ContentItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var result = "<section ";

        if (!string.IsNullOrWhiteSpace(item.Alignment))
            result += $"class=\"content-{item.Alignment}\"";
        
        result += ">";
        return result;
    }

    public static void AddFromSlide(this List<ContentItem> images, Slide slideValue, ContentItem? presentationDefaultBackgroundContent = null)
    {
        ArgumentNullException.ThrowIfNull(images);
        ArgumentNullException.ThrowIfNull(slideValue);

        var backgroundContent = slideValue.BackgroundContent ?? presentationDefaultBackgroundContent;
        if (backgroundContent != null)
            images.AddIfNotPresent(backgroundContent);

        // Add additional content item images to images collection
        foreach (var contentItem in slideValue.ContentItems)
            if (contentItem.Value.IsImage())
                images.AddIfNotPresent(contentItem.Value);
    }

    public static void AddIfNotPresent(this List<ContentItem> items, ContentItem item)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(item);

        if (!items.Any(i => i.Id == item.Id))
            items.Add(item);
    }

    public static string RelativePathToImage(this ContentItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return $"img/{item.Id.ToString()}{System.IO.Path.GetExtension(item.FileName)}";
    }

    /// <summary>
    /// Renders a text content item as HTML, optionally marking it as a RevealJS fragment
    /// so that it is only displayed when the slide is advanced.
    /// </summary>
    public static string AsFragmentHtml(this ContentItem contentItem, Markdig.MarkdownPipeline pipeline, bool isFragment)
    {
        ArgumentNullException.ThrowIfNull(contentItem);

        var markdown = contentItem.Content.AsString();
        if (isFragment)
            markdown = $"{{.fragment}}\r\n{markdown}";

        return Markdig.Markdown.ToHtml(markdown, pipeline);
    }

    public static bool IsText(this ContentItem contentItem)
    {
        ArgumentNullException.ThrowIfNull(contentItem);
        return contentItem.ContentType.StartsWith("text", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsImage(this ContentItem contentItem)
    {
        ArgumentNullException.ThrowIfNull(contentItem);
        return contentItem.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase);
    }

}
