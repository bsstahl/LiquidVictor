using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.Jupyter.Extensions;

public static class ContentItemExtensions
{
    public static string AsImageSource(this ContentItem contentItem)
    {
        ArgumentNullException.ThrowIfNull(contentItem);

        if (contentItem.Content.Length == 0)
            return contentItem.FileName;

        return $"data:{contentItem.ContentType};base64,{contentItem.Content.AsBase64String()}";
    }
}
