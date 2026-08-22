using LiquidVictor.Entities;

namespace LiquidVictor.Output.RevealJs.Generator.Extensions;

internal static class ContentItemExtensions
{
    internal static void AddImages(this IEnumerable<ContentItem> images, string targetPath)
    {
        string folderPath = System.IO.Path.Combine(targetPath, "img");
        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);

        foreach (var contentItem in images)
        {
            var fileName = $"{contentItem.Id.ToString()}{Path.GetExtension(contentItem.FileName)}";
            var filePath = System.IO.Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllBytes(filePath, contentItem.Content);
        }
    }


}
