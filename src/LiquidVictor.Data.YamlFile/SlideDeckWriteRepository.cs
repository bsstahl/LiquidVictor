using System;
using System.IO;
using System.Linq;
using LiquidVictor.Extensions;

namespace LiquidVictor.Data.YamlFile;

public class SlideDeckWriteRepository : Interfaces.ISlideDeckWriteRepository
{
    string _sourceFolderPath;

    public SlideDeckWriteRepository(string sourceFolderPath)
    {
        _sourceFolderPath = System.IO.Path.GetFullPath(sourceFolderPath);
    }

    public void SaveSlideDeck(Entities.SlideDeck slideDeck)
    {
        var sd = new SlideDeck()
        {
            Id = slideDeck.Id.ToString(),
            AspectRatio = slideDeck.AspectRatio.ToString(),
            Presenter = slideDeck.Presenter,
            PrintLinkText = slideDeck.PrintLinkText,
            SubTitle = slideDeck.SubTitle,
            ThemeName = slideDeck.ThemeName,
            Title = slideDeck.Title,
            Transition = slideDeck.Transition.ToString(),
            Format = slideDeck.Format.ToString(),
            SlideIds = slideDeck.Slides.OrderBy(s => s.Key).Select(s => new ChildId(s.Value.Id, s.Value.Title)).ToArray()
        };

        var slideDeckFileName = GetSlideDeckFileName(slideDeck);
        string slideDeckPath = System.IO.Path.Combine(_sourceFolderPath, $"SlideDecks\\{slideDeckFileName}.yaml");

        // Create folder structure if necessary
        if (!System.IO.Directory.Exists(_sourceFolderPath))
            System.IO.Directory.CreateDirectory(_sourceFolderPath);

        // Write SlideDeck file
        File.WriteAllText(slideDeckPath, sd.ToString());

        // Write Slides
        // TODO: Deduplicate (in case a slide is used more than once in a presentation)
        foreach (var s in slideDeck.Slides)
            this.SaveSlide(s.Value);
    }

    public void SaveSlide(Entities.Slide s)
    {
        string slidesPath = System.IO.Path.Combine(_sourceFolderPath, "Slides");
        if (!System.IO.Directory.Exists(slidesPath))
            System.IO.Directory.CreateDirectory(slidesPath);

        var slide = new Slide()
        {
            BackgroundContent = s.BackgroundContent?.Id.ToString(),
            Layout = s.Layout.ToString(),
            NeverFullScreen = s.NeverFullScreen,
            Notes = s.Notes,
            Title = s.Title,
            TransitionIn = s.TransitionIn.ToString(),
            TransitionOut = s.TransitionOut.ToString(),
            ContentItemIds = s.ContentItems.OrderBy(ci => ci.Key).Select(ci => new ChildId(ci.Value.Id, ci.Value.Title)).ToArray()
        };

        // Write slide file
        string slidePath = System.IO.Path.Combine(slidesPath, $"{s.Id}.yaml");
        File.WriteAllText(slidePath, slide.ToString());

        // Write ContentItems
        foreach (var ci in s.ContentItems)
            SaveContentItem(ci.Value);

        if (slide.BackgroundContent is not null)
            SaveContentItem(s.BackgroundContent);
    }

    public void SaveContentItem(Entities.ContentItem contentItem)
    {
        var ci = new ContentItem()
        {
            Alignment = contentItem.Alignment,
            ContentType = contentItem.ContentType,
            EncodedContent = contentItem.Content.EncodeContent(contentItem.ContentType),
            FileName = contentItem.FileName,
            Title = contentItem.Title
        };

        string contentItemsPath = System.IO.Path.Combine(_sourceFolderPath, "ContentItems");
        if (!System.IO.Directory.Exists(contentItemsPath))
            System.IO.Directory.CreateDirectory(contentItemsPath);

        // Write ContentItem file
        string contentItemPath = System.IO.Path.Combine(contentItemsPath, $"{contentItem.Id}.yaml");
        File.WriteAllText(contentItemPath, ci.ToString());
    }

    private string GetSlideDeckFileName(Entities.SlideDeck slideDeck)
    {
        // If the slide deck already exists (per the id), use that filename
        // otherwise, use a filename generated from the title and format of the presentation
        string result = _sourceFolderPath.FindFileWithId(slideDeck.Id);
        if (string.IsNullOrWhiteSpace(result))
        {
            var fullTitle = $"{slideDeck.Title} {slideDeck.SubTitle}".Trim();
            result = $"{fullTitle}-{slideDeck.Format}".Clean();
            var filePath = System.IO.Path.Combine(_sourceFolderPath, $"{result}.yaml");
            if (File.Exists(filePath))
                throw new InvalidOperationException($"SlideDeck already exists at '{filePath}'");
        }
        return result;
    }

}
